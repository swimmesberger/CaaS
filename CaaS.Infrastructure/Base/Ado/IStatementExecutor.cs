using CaaS.Infrastructure.Base.Ado.Model;

namespace CaaS.Infrastructure.Base.Ado; 

public interface IStatementExecutor {
    Task<object?> QueryScalarAsync(Statement statement, CancellationToken cancellationToken = default);
    Task<object?> QueryScalarAsync(MaterializedStatements statement, CancellationToken cancellationToken = default);
    Task<List<T>> QueryAsync<T>(Statement statement, 
            RowMapper<T> mapper, CancellationToken cancellationToken = default);
    IAsyncEnumerable<T> StreamAsync<T>(Statement statement, 
            RowMapper<T> mapper, CancellationToken cancellationToken = default);
    IAsyncEnumerable<T> StreamAsync<T>(MaterializedStatements statement, 
        RowMapper<T> mapper, CancellationToken cancellationToken = default);
    Task<int> ExecuteAsync(Statement statement, 
            CancellationToken cancellationToken = default);
}