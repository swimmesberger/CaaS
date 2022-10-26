using CaaS.Core.Entities.Base;
using CaaS.Core.Exceptions;
using CaaS.Core.Repositories.Base;
using CaaS.Infrastructure.Ado;
using CaaS.Infrastructure.Repositories.Base.Mapping;

namespace CaaS.Infrastructure.Repositories.Base;

public abstract class Repository<T> : IRepository<T> where T : Entity, new() {
    private readonly IStatementExecutor _statementExecutor;
    
    protected IStatementGenerator<T> StatementGenerator { get; }
    protected IRecordMapper<T> RecordMapper => StatementGenerator.RecordMapper;

    public Repository(IStatementExecutor statementExecutor, IStatementGenerator<T> statementGenerator) {
        _statementExecutor = statementExecutor;
        StatementGenerator = statementGenerator;
    }

    public async Task<IReadOnlyList<T>> FindAllAsync(CancellationToken cancellationToken = default) {
        return await QueryAsync(cancellationToken: CancellationToken.None);
    }

    public async Task<T?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) {
        return (await QueryByPropertyAsync(nameof(IEntityBase.Id), id, cancellationToken)).FirstOrDefault();
    }
    
    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default) {
        var changedCount = await _statementExecutor.ExecuteAsync(StatementGenerator.CreateInsert(entity), cancellationToken);
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
        var statement = StatementGenerator.CreateUpdate(entity, origRowVersion);
        var changedCount = await _statementExecutor.ExecuteAsync(statement, cancellationToken);
        if (changedCount == 0) {
            throw new CaasUpdateConcurrencyDbException();
        }
        return entity;
    }
    
    public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default) {
        var changedCount = await _statementExecutor.ExecuteAsync(StatementGenerator.CreateDelete(entity), cancellationToken);
        if (changedCount == 0) {
            throw new CaasInsertDbException();
        }
    }
    
    public async Task<long> CountAsync(CancellationToken cancellationToken = default) {
        return (long)(await _statementExecutor
                .QueryScalarAsync(StatementGenerator.CreateCount(), cancellationToken) ?? throw new InvalidOperationException());
    }
    
    protected Task<List<T>> QueryByPropertyAsync(string propertyName, object value, CancellationToken cancellationToken = default) 
        => QueryAsync(new QueryParameter(RecordMapper.ByPropertyName().MapName(propertyName), value), cancellationToken);

    protected Task<List<T>> QueryAsync(QueryParameter parameter, CancellationToken cancellationToken = default) 
        => QueryAsync(new[] { parameter }, cancellationToken);

    protected async Task<List<T>> QueryAsync(IEnumerable<QueryParameter>? parameters = null,
            CancellationToken cancellationToken = default) {
        var statement = StatementGenerator.CreateFind(parameters);
        statement = await PostProcessStatement(statement, cancellationToken);
        return await _statementExecutor.QueryAsync(statement, RecordMapper.EntityFromRecord, cancellationToken: cancellationToken);
    }

    protected virtual Task<Statement> PostProcessStatement(Statement statement, 
            CancellationToken cancellationToken = default) => Task.FromResult(statement);
}