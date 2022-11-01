﻿namespace CaaS.Infrastructure.Ado; 

public interface IStatementExecutor {
    Task<object?> QueryScalarAsync(Statement statement, CancellationToken cancellationToken = default);
    
    Task<List<T>> QueryAsync<T>(Statement statement, 
            RowMapper<T> mapper, CancellationToken cancellationToken = default);
    Task<int> ExecuteAsync(Statement statement, 
            CancellationToken cancellationToken = default);
}