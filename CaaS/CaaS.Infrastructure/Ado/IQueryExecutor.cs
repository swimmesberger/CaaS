namespace CaaS.Infrastructure.Ado; 

public interface IQueryExecutor {
    Task<List<T>> QueryAsync<T>(Statement statement, 
            RowMapper<T> mapper, CancellationToken cancellationToken = default);
    Task<int> ExecuteAsync(Statement statement, 
            CancellationToken cancellationToken = default);
}