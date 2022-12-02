using System.Data.Common;
using System.Runtime.CompilerServices;
using CaaS.Core.Base;
using CaaS.Core.Base.Exceptions;
using CaaS.Core.Base.Tenant;
using CaaS.Infrastructure.Base.Ado.Model;
using CaaS.Infrastructure.Base.Di;
using CaaS.Infrastructure.Base.Mapping;
using CaaS.Infrastructure.Base.Model;

namespace CaaS.Infrastructure.Base.Ado.Impl;

public sealed class GenericDao<T> : IDao<T>, IDataRecordProvider<T> where T : DataModel, new() {
    private readonly IStatementExecutor _statementExecutor;
    private readonly IStatementGenerator<T> _statementGenerator;
    private readonly IDateTimeOffsetProvider _timeProvider;
    private readonly ITenantIdAccessor? _tenantService;
    private readonly ITenantIdProvider<T>? _tenantIdProvider;

    public IDataRecordMapper<T> DataRecordMapper => _statementGenerator.DataRecordMapper;
    
    public GenericDao(IStatementExecutor statementExecutor, 
            IStatementGenerator<T> statementGenerator, 
            IDateTimeOffsetProvider timeProvider, 
            IServiceProvider<ITenantIdAccessor>? tenantService = null) {
        tenantService ??= IServiceProvider<ITenantIdAccessor>.Empty;
        _statementExecutor = statementExecutor;
        _statementGenerator = statementGenerator;
        _timeProvider = timeProvider;
        if (DataRecordMapper is ITenantIdProvider<T> tenantIdProvider) {
            _tenantIdProvider = tenantIdProvider;
            _tenantService = tenantService.GetRequiredService();
        }
    }

    public IAsyncEnumerable<T> FindAllAsync(CancellationToken cancellationToken = default) {
        return QueryAsync(cancellationToken: cancellationToken);
    }

    public IAsyncEnumerable<T> FindBy(StatementParameters parameters, CancellationToken cancellationToken = default) {
        return QueryAsync(parameters.MapParameterNames(name => DataRecordMapper.ByPropertyName().MapName(name)), cancellationToken);
    }
    
    public IAsyncEnumerable<TValue> FindScalarBy<TValue>(StatementParameters parameters, CancellationToken cancellationToken = default) {
        if (parameters.Select.Properties == null || parameters.Select.Properties.Count > 1) {
            throw new CaasDbException("Invalid select for FindScalarBy");
        }
        parameters = parameters
            .MapParameterNames(name => DataRecordMapper.ByPropertyName().MapName(name));
        return QueryAsync((record, token) => record.GetValueAsync<TValue>(0, token), parameters, cancellationToken);
    }

    public IAsyncEnumerable<Guid> FindIdsBy(StatementParameters parameters, CancellationToken cancellationToken = default) {
        parameters = parameters
            .WithSelect(nameof(DataModel.Id))
            .MapParameterNames(name => DataRecordMapper.ByPropertyName().MapName(name));
        return QueryAsync((record, _) => new ValueTask<Guid>(record.GetGuid(0)), parameters, cancellationToken);
    }

    public async Task<long> CountAsync(StatementParameters? parameters = null, CancellationToken cancellationToken = default) {
        parameters ??= StatementParameters.Empty;
        return (long)(await QueryScalarAsync(_statementGenerator.CreateCount(parameters), cancellationToken) ?? throw new InvalidOperationException());
    }
    
    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default) {
        var changedCount = await ExecuteAsync(_statementGenerator.CreateInsert(entity), cancellationToken);
        if (changedCount == 0) {
            throw new CaasInsertDbException();
        }
        return entity;
    }
    
    public async Task<IReadOnlyCollection<T>> AddAsync(IReadOnlyCollection<T> entities, CancellationToken cancellationToken = default) {
        if (entities.Count == 0) return entities;
        var changedCount = await ExecuteAsync(_statementGenerator.CreateInsert(entities), cancellationToken);
        if (changedCount == 0) {
            throw new CaasInsertDbException();
        }
        return entities;
    }

    public async Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default) {
        var origRowVersion = entity.RowVersion;
        entity = entity with {
            RowVersion = origRowVersion+1,
            LastModificationTime = _timeProvider.GetNow()
        };
        var statement = _statementGenerator.CreateUpdate(entity, origRowVersion);
        var changedCount = await ExecuteAsync(statement, cancellationToken);
        if (changedCount == 0) {
            throw new CaasUpdateConcurrencyDbException();
        }
        return entity;
    }

    public async Task<IReadOnlyCollection<T>> UpdateAsync(IReadOnlyCollection<T> entities, CancellationToken cancellationToken = default) {
        if (entities.Count == 0) return entities;
        var versionedEntities = entities.Select(e => {
            var origRowVersion = e.RowVersion;
            return new VersionedEntity<T>(e with {
                RowVersion = origRowVersion + 1,
                LastModificationTime = _timeProvider.GetNow()
            }, origRowVersion);
        }).ToList();
        var statement = _statementGenerator.CreateUpdate(versionedEntities);
        var changedCount = await ExecuteAsync(statement, cancellationToken);
        if (changedCount < versionedEntities.Count) {
            throw new CaasUpdateConcurrencyDbException();
        }
        return versionedEntities.Select(e => e.Entity).ToList();
    }

    public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default) {
        var changedCount = await ExecuteAsync(_statementGenerator.CreateDelete(entity), cancellationToken);
        if (changedCount == 0) {
            throw new CaasInsertDbException();
        }
    }

    public async Task DeleteAsync(IReadOnlyCollection<T> entities, CancellationToken cancellationToken = default) {
        if (entities.Count == 0) return;
        var changedCount = await ExecuteAsync(_statementGenerator.CreateDelete(entities), cancellationToken);
        if (changedCount == 0) {
            throw new CaasInsertDbException();
        }
    }

    private async Task<object?> QueryScalarAsync(Statement statement, CancellationToken cancellationToken = default) {
        try {
            return await _statementExecutor.QueryScalarAsync(statement, cancellationToken);
        } catch (DbException ex) {
            throw new CaasDbException("Failed to execute statement", ex);
        }
    }

    private async Task<int> ExecuteAsync(Statement statement, CancellationToken cancellationToken = default) {
        try {
            return await _statementExecutor.ExecuteAsync(statement, cancellationToken);
        } catch (DbException ex) {
            throw new CaasDbException("Failed to execute statement", ex);
        }
    }

    private IAsyncEnumerable<T> QueryAsync(StatementParameters? parameters = null, 
            CancellationToken cancellationToken = default) => QueryAsync(DataRecordMapper.EntityFromRecordAsync, parameters, cancellationToken);
    
    private async IAsyncEnumerable<TR> QueryAsync<TR>(RowMapper<TR> rowMapper, StatementParameters? parameters = null, 
        [EnumeratorCancellation] CancellationToken cancellationToken = default) {
        parameters ??= StatementParameters.Empty;
        var statement = _statementGenerator.CreateFind(parameters);
        statement = PostProcessStatement(statement);
        IAsyncEnumerable<TR> enumerable;
        try {
            enumerable = _statementExecutor
                .StreamAsync(statement, rowMapper, cancellationToken: cancellationToken);
        } catch (DbException ex) {
            throw new CaasDbException("Failed to execute statement", ex);
        }
        await foreach (var dataModel in enumerable.WithCancellation(cancellationToken)) {
            yield return dataModel;
        }
    }

    private Statement PostProcessStatement(Statement statement) {
        if (_tenantService == null || _tenantIdProvider == null) {
            return statement;
        }
        var tenantId = _tenantService!.GetTenantGuid();
        var tenantIdColumnName = DataRecordMapper.ByPropertyName().MapName(_tenantIdProvider!.TenantIdPropertyName);
        return statement.AddWhereParameter(tenantIdColumnName, tenantId);
    }
}