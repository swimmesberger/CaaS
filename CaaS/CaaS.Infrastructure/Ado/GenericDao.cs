using System.Runtime.CompilerServices;
using CaaS.Core.Exceptions;
using CaaS.Core.Tenant;
using CaaS.Infrastructure.Ado.Base;
using CaaS.Infrastructure.Ado.Model;
using CaaS.Infrastructure.DataMapping;
using CaaS.Infrastructure.DataMapping.Base;
using CaaS.Infrastructure.DataModel.Base;
using CaaS.Infrastructure.Di;

namespace CaaS.Infrastructure.Ado;

public sealed class GenericDao<T> : IDao<T> where T : DataModel.Base.DataModel, new() {
    private readonly IStatementExecutor _statementExecutor;
    private readonly IStatementGenerator<T> _statementGenerator;
    private readonly ITenantService? _tenantService;
    private readonly ITenantIdProvider<T>? _tenantIdProvider;

    private IDataRecordMapper<T> DataRecordMapper => _statementGenerator.DataRecordMapper;

    public GenericDao(IStatementExecutor statementExecutor, IStatementGenerator<T> statementGenerator, 
            IServiceProvider<ITenantService>? tenantService = null) {
        _statementExecutor = statementExecutor;
        _statementGenerator = statementGenerator;
        if (DataRecordMapper is ITenantIdProvider<T> tenantIdProvider) {
            if (tenantService == null) throw new ArgumentException("No tenant service", nameof(tenantService));
            _tenantIdProvider = tenantIdProvider;
            _tenantService = tenantService.GetRequiredService();
        }
    }

    public IAsyncEnumerable<T> FindAllAsync(CancellationToken cancellationToken = default) {
        return QueryAsync(cancellationToken: cancellationToken);
    }

    public async Task<T?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) {
        return await FindBy(StatementParameters.CreateWhere(nameof(IDataModelBase.Id), id), cancellationToken).FirstOrDefaultAsync(cancellationToken);
    }

    public IAsyncEnumerable<T> FindByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default) {
        return FindBy(StatementParameters.CreateWhere(nameof(IDataModelBase.Id), ids), cancellationToken);
    }

    public IAsyncEnumerable<T> FindBy(StatementParameters parameters, CancellationToken cancellationToken = default) {
        return QueryAsync(parameters.MapParameterNames(name => DataRecordMapper.ByPropertyName().MapName(name)), cancellationToken);
    }

    public async Task<long> CountAsync(CancellationToken cancellationToken = default) {
        return (long)(await _statementExecutor
                .QueryScalarAsync(_statementGenerator.CreateCount(), cancellationToken) ?? throw new InvalidOperationException());
    }
    
    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default) {
        var changedCount = await _statementExecutor.ExecuteAsync(_statementGenerator.CreateInsert(entity), cancellationToken);
        if (changedCount == 0) {
            throw new CaasInsertDbException();
        }
        return entity;
    }
    
    public async Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default) {
        var origRowVersion = entity.RowVersion;
        entity = entity with {
            RowVersion = origRowVersion+1,
            LastModificationTime = DateTimeOffset.UtcNow
        };
        var statement = _statementGenerator.CreateUpdate(entity, origRowVersion);
        var changedCount = await _statementExecutor.ExecuteAsync(statement, cancellationToken);
        if (changedCount == 0) {
            throw new CaasUpdateConcurrencyDbException();
        }
        return entity;
    }
    
    public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default) {
        var changedCount = await _statementExecutor.ExecuteAsync(_statementGenerator.CreateDelete(entity), cancellationToken);
        if (changedCount == 0) {
            throw new CaasInsertDbException();
        }
    }
    
    private async IAsyncEnumerable<T> QueryAsync(StatementParameters? parameters = null, 
            [EnumeratorCancellation] CancellationToken cancellationToken = default) {
        parameters ??= StatementParameters.Empty;
        var statement = _statementGenerator.CreateFind(parameters);
        statement = await PostProcessStatement(statement, cancellationToken);
        var enumerable = _statementExecutor
                .StreamAsync(statement, DataRecordMapper.EntityFromRecord, cancellationToken: cancellationToken);
        await foreach (var dataModel in enumerable.WithCancellation(cancellationToken)) {
            yield return dataModel;
        }
    }

    private ValueTask<Statement> PostProcessStatement(Statement statement, 
            CancellationToken cancellationToken = default) {
        if (_tenantService == null || _tenantIdProvider == null) {
            return new ValueTask<Statement>(statement);
        }
        return PostProcessStatementWithTenant(statement, cancellationToken);
    }

    private async ValueTask<Statement> PostProcessStatementWithTenant(Statement statement,
            CancellationToken cancellationToken = default) {
        var tenantId = await _tenantService!.GetTenantIdAsync(cancellationToken);
        var tenantIdColumnName = DataRecordMapper.ByPropertyName().MapName(_tenantIdProvider!.TenantIdPropertyName);
        return statement.AddWhereParameter(tenantIdColumnName, tenantId);
    }
}