using System.Data.Common;
using System.Runtime.CompilerServices;
using CaaS.Core.Base;
using CaaS.Core.Base.Exceptions;
using CaaS.Core.Base.Tenant;
using CaaS.Infrastructure.Base.Ado.Model;
using CaaS.Infrastructure.Base.Ado.Query;
using CaaS.Infrastructure.Base.Ado.Query.Parameters;
using CaaS.Infrastructure.Base.Di;
using CaaS.Infrastructure.Base.Mapping;
using CaaS.Infrastructure.Base.Model;

namespace CaaS.Infrastructure.Base.Ado.Impl;

public sealed class GenericDao<T> : IDao<T>, IHasMetadataProvider where T : DataModel, new() {
    private readonly IStatementExecutor _statementExecutor;
    private readonly IStatementMaterializer _statementMaterializer;
    private readonly IStatementGenerator<T> _statementGenerator;
    private readonly ISystemClock _timeProvider;
    private readonly ITenantIdAccessor? _tenantService;
    private readonly ITenantIdProvider<T>? _tenantIdProvider;
    
    public IRecordMetadataProvider MetadataProvider => _statementGenerator.DataRecordMapper.MetadataProvider;

    public GenericDao(IStatementExecutor statementExecutor,
            IStatementMaterializer statementMaterializer,
            IStatementGenerator<T> statementGenerator,
            ISystemClock timeProvider,
            IServiceProvider<ITenantIdAccessor>? tenantService = null) {
        tenantService ??= IServiceProvider<ITenantIdAccessor>.Empty;
        _statementExecutor = statementExecutor;
        _statementMaterializer = statementMaterializer;
        _statementGenerator = statementGenerator;
        _timeProvider = timeProvider;
        if (_statementGenerator.DataRecordMapper is ITenantIdProvider<T> tenantIdProvider) {
            _tenantIdProvider = tenantIdProvider;
            _tenantService = tenantService.GetRequiredService();
        }
    }

    public IAsyncEnumerable<T> FindAllAsync(CancellationToken cancellationToken = default) {
        return QueryAsync(cancellationToken: cancellationToken);
    }

    public IAsyncEnumerable<T> FindBy(StatementParameters parameters, CancellationToken cancellationToken = default) {
        return QueryAsync(parameters, cancellationToken);
    }
    
    public IAsyncEnumerable<TValue> FindScalarBy<TValue>(StatementParameters parameters, CancellationToken cancellationToken = default) {
        if (parameters.SelectParameters.Properties == null || parameters.SelectParameters.Properties.Count > 1) {
            throw new CaasDbException("Invalid select for FindScalarBy");
        }
        return QueryAsync((record, token) => record.GetValueAsync<TValue>(0, token), parameters, cancellationToken);
    }

    public async Task<long> CountAsync(StatementParameters? parameters = null, CancellationToken cancellationToken = default) {
        parameters ??= StatementParameters.Empty;
        long? count = await QueryScalarAsync(_statementGenerator.CreateCount(parameters), cancellationToken);
        return count ?? throw new InvalidOperationException();
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
            LastModificationTime = _timeProvider.UtcNow
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
                LastModificationTime = _timeProvider.UtcNow
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
            throw new CaasDeleteDbException();
        }
    }

    public async Task DeleteAsync(IReadOnlyCollection<T> entities, CancellationToken cancellationToken = default) {
        if (entities.Count == 0) return;
        var changedCount = await ExecuteAsync(_statementGenerator.CreateDelete(entities), cancellationToken);
        if (changedCount == 0) {
            throw new CaasDeleteDbException();
        }
    }

    public IReadOnlyDictionary<string, object?> ReadPropertiesFromModel(T model, IEnumerable<string> properties) {
        var recordValues = _statementGenerator.DataRecordMapper.RecordFromEntity(model).ByPropertyName();
        var propertyValuePairs = new Dictionary<string, object?>();
        foreach (var property in properties) {
            propertyValuePairs[property] = recordValues.GetObject(property);
        }
        return propertyValuePairs;
    }

    private async Task<TResult?> QueryScalarAsync<TResult>(Statement<TResult> statement, CancellationToken cancellationToken = default) {
        try {
            return await _statementExecutor.QueryScalarAsync(_statementMaterializer.MaterializeStatement(statement), cancellationToken);
        } catch (DbException ex) {
            throw new CaasDbException("Failed to execute statement", ex);
        }
    }

    private Task<int> ExecuteAsync(Statement statement, CancellationToken cancellationToken = default) {
        return ExecuteAsync(new StatementBatch(statement), cancellationToken);
    }

    private async Task<int> ExecuteAsync(StatementBatch batch, CancellationToken cancellationToken = default) {
        try {
            return await _statementExecutor.ExecuteAsync(_statementMaterializer.MaterializeBatch(batch), cancellationToken);
        } catch (DbException ex) {
            throw new CaasDbException("Failed to execute statement", ex);
        }
    }

    private IAsyncEnumerable<T> QueryAsync(StatementParameters? parameters = null, CancellationToken cancellationToken = default) {
        parameters ??= StatementParameters.Empty;
        var statement = _statementGenerator.CreateFind(parameters);
        return QueryAsync(statement, cancellationToken);
    }
    
    private IAsyncEnumerable<TR> QueryAsync<TR>(RowMapper<TR> rowMapper, StatementParameters? parameters = null, 
        CancellationToken cancellationToken = default) {
        parameters ??= StatementParameters.Empty;
        var statement = _statementGenerator.CreateFind(parameters, rowMapper);
        return QueryAsync(statement, cancellationToken);
    }
    
    private async IAsyncEnumerable<TR> QueryAsync<TR>(Statement<TR> statement, [EnumeratorCancellation] CancellationToken cancellationToken = default) {
        statement = PostProcessStatement(statement);
        IAsyncEnumerable<TR> enumerable;
        try {
            enumerable = _statementExecutor.StreamAsync(_statementMaterializer.MaterializeStatement(statement), cancellationToken: cancellationToken);
        } catch (DbException ex) {
            throw new CaasDbException("Failed to execute statement", ex);
        }
        await foreach (var dataModel in enumerable.WithCancellation(cancellationToken)) {
            yield return dataModel;
        }
    }

    private Statement<TResult> PostProcessStatement<TResult>(Statement<TResult> statement) {
        if (_tenantService == null || _tenantIdProvider == null) {
            return statement;
        }
        var tenantId = _tenantService!.GetTenantGuid();
        return statement.AddWhereParameter(_tenantIdProvider!.TenantIdPropertyName, tenantId);
    }
}