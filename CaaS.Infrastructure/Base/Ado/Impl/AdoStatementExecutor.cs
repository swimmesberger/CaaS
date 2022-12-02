using System.Data.Common;
using System.Runtime.CompilerServices;
using CaaS.Infrastructure.Base.Ado.Model;
using CaaS.Infrastructure.Base.Mapping;

namespace CaaS.Infrastructure.Base.Ado.Impl; 

public class AdoStatementExecutor : IStatementExecutor {
    private readonly IHasConnectionProvider _unitOfWorkManager;

    public AdoStatementExecutor(IHasConnectionProvider unitOfWorkManager) {
        _unitOfWorkManager = unitOfWorkManager;
    }

    public Task<TResult?> QueryScalarAsync<TResult>(Statement<TResult> statement, CancellationToken cancellationToken = default) {
        return QueryScalarAsync(statement.Materialize(), statement.MapResult, cancellationToken);
    }

    public Task<object?> QueryScalarAsync(MaterializedStatements statement, CancellationToken cancellationToken = default) {
        ValueTask<object> RowMapper(DbDataReader record, CancellationToken token) => record.GetValueAsync<object>(0, token);
        return QueryScalarAsync(statement, RowMapper, cancellationToken);
    }
    
    private async Task<TResult?> QueryScalarAsync<TResult>(MaterializedStatements statement, RowMapper<TResult> rowMapper, CancellationToken cancellationToken = default) {
        await using var connectionProvider = _unitOfWorkManager.ConnectionProvider;
        await using var cmd = await CreateCommand(statement, connectionProvider, cancellationToken);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        var read = await reader.ReadAsync(cancellationToken);
        return read && reader.FieldCount != 0 ? await rowMapper.Invoke(reader, cancellationToken) : default;
    }

    public Task<List<T>> QueryAsync<T>(Statement<T> statement, CancellationToken cancellationToken = default)
        => QueryAsync(statement, statement.MapResult, cancellationToken);

    private async Task<List<T>> QueryAsync<T>(Statement statement, RowMapper<T> mapper, CancellationToken cancellationToken = default) {
        await using var connectionProvider = _unitOfWorkManager.ConnectionProvider;
        await using var cmd = await CreateCommand(statement, connectionProvider, cancellationToken);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        var items = new List<T>();
        while (await reader.ReadAsync(cancellationToken)) {
            items.Add(await mapper(reader, cancellationToken));
        }
        return items;
    }

    public IAsyncEnumerable<T> StreamAsync<T>(Statement<T> statement, CancellationToken cancellationToken = default)
        => StreamAsync(statement, statement.MapResult, cancellationToken);

    private IAsyncEnumerable<T> StreamAsync<T>(Statement statement, RowMapper<T> mapper, 
            CancellationToken cancellationToken = default) => StreamAsync(statement.Materialize(), mapper, cancellationToken);
    
    public async IAsyncEnumerable<T> StreamAsync<T>(MaterializedStatements statement, RowMapper<T> mapper, 
            [EnumeratorCancellation] CancellationToken cancellationToken = default) {
        await using var connectionProvider = _unitOfWorkManager.ConnectionProvider;
        await using var cmd = await CreateCommand(statement, connectionProvider, cancellationToken);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken)) {
            yield return await mapper(reader, cancellationToken);
        }
    }
    
    public async Task<int> ExecuteAsync(Statement statement,
            CancellationToken cancellationToken = default) {
        if (statement == Statement.Empty) return 0;
        await using var connectionProvider = _unitOfWorkManager.ConnectionProvider;
        await using var cmd = await CreateCommand(statement, connectionProvider, cancellationToken);
        var result = await cmd.ExecuteNonQueryAsync(cancellationToken);
        return result;
    }

    private Task<DbBatch> CreateCommand(Statement statement, IConnectionProvider connectionProvider,
        CancellationToken cancellationToken = default) {
        return CreateCommand(statement.Materialize(), connectionProvider, cancellationToken);
    }
    
    private async Task<DbBatch> CreateCommand(MaterializedStatements materializedStatements, IConnectionProvider connectionProvider, 
        CancellationToken cancellationToken = default) {
        var dbConnection = await connectionProvider
                .GetDbConnectionAsync(cancellationToken: cancellationToken);
        var batch = dbConnection.CreateBatch();
        batch.Connection = dbConnection;
        batch.Transaction = connectionProvider.CurrentTransaction;
        foreach (var materializedStatement in materializedStatements.Statements) {
            var cmd = batch.CreateBatchCommand();
            cmd.CommandText = materializedStatement.Sql;
            foreach (var queryParameter in materializedStatement.Parameters) {
                var parameter = connectionProvider.Factory.CreateParameter();
                parameter = parameter.SetParameter(queryParameter.ParameterName, queryParameter.TypedValue);
                cmd.Parameters.Add(parameter);
            }
            batch.BatchCommands.Add(cmd);
        }
        return batch;
    }
}