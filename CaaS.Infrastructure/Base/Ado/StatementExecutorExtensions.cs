using CaaS.Infrastructure.Base.Ado.Query;
using CaaS.Infrastructure.Base.Mapping;

namespace CaaS.Infrastructure.Base.Ado; 

public static class StatementExecutorExtensions {
    private static RowMapper<T> FirstFieldMapper<T>() => (record, token) => record.GetValueAsync<T>(0, token);

    public static Task<T?> QueryScalarAsync<T>(this IStatementExecutor statementExecutor, MaterializedStatements statements, 
        CancellationToken cancellationToken = default) {
        return statementExecutor.QueryScalarAsync(new MaterializedStatements<T>(statements, FirstFieldMapper<T>()), cancellationToken);
    }
    
    public static Task<T?> QueryScalarAsync<T>(this IStatementExecutor statementExecutor, MaterializedStatement statement, 
        CancellationToken cancellationToken = default) {
        return statementExecutor.QueryScalarAsync(new MaterializedStatements<T>(statement, FirstFieldMapper<T>()), cancellationToken);
    }
    
    public static Task<TResult?> QueryScalarAsync<TResult>(this IStatementExecutor statementExecutor, MaterializedStatement<TResult> statement, 
        CancellationToken cancellationToken = default) {
        return statementExecutor.QueryScalarAsync(new MaterializedStatements<TResult>(statement, statement.RowMapper), 
            cancellationToken);
    }

    public static IAsyncEnumerable<T> StreamAsync<T>(this IStatementExecutor statementExecutor, MaterializedStatement<T> statement, CancellationToken cancellationToken = default)
        => statementExecutor.StreamAsync(new MaterializedStatements<T>(statement, statement.RowMapper), cancellationToken);
    
    public static IAsyncEnumerable<T> StreamAsync<T>(this IStatementExecutor statementExecutor, MaterializedStatements statements, 
        RowMapper<T> mapper, CancellationToken cancellationToken = default)
        => statementExecutor.StreamAsync(new MaterializedStatements<T>(statements, mapper), cancellationToken);
    
    public static IAsyncEnumerable<T> StreamAsync<T>(this IStatementExecutor statementExecutor, MaterializedStatement statement, 
        RowMapper<T> mapper, CancellationToken cancellationToken = default)
        => statementExecutor.StreamAsync(new MaterializedStatements<T>(statement, mapper), cancellationToken);
}