using System.Data.Common;
using System.Runtime.CompilerServices;
using CaaS.Infrastructure.Ado.Base;
using CaaS.Infrastructure.Ado.Model;

namespace CaaS.Infrastructure.Ado; 

public class AdoStatementExecutor : IStatementExecutor {
    private readonly IHasConnectionProvider _unitOfWorkManager;

    public AdoStatementExecutor(IHasConnectionProvider unitOfWorkManager) {
        _unitOfWorkManager = unitOfWorkManager;
    }

    public async Task<object?> QueryScalarAsync(Statement statement, CancellationToken cancellationToken = default) {
        await using var connectionProvider = _unitOfWorkManager.ConnectionProvider;
        await using var cmd = await CreateCommand(statement, connectionProvider, cancellationToken);
        return await cmd.ExecuteScalarAsync(cancellationToken);
    }

    public async Task<List<T>> QueryAsync<T>(Statement statement,
            RowMapper<T> mapper,
            CancellationToken cancellationToken = default) {
        await using var connectionProvider = _unitOfWorkManager.ConnectionProvider;
        await using var cmd = await CreateCommand(statement, connectionProvider, cancellationToken);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        var items = new List<T>();
        while (await reader.ReadAsync(cancellationToken)) {
            items.Add(mapper(reader));
        }
        return items;
    }

    public async IAsyncEnumerable<T> StreamAsync<T>(Statement statement, RowMapper<T> mapper, 
            [EnumeratorCancellation] CancellationToken cancellationToken = default) {
        await using var connectionProvider = _unitOfWorkManager.ConnectionProvider;
        await using var cmd = await CreateCommand(statement, connectionProvider, cancellationToken);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken)) {
            yield return mapper(reader);
        }
    }

    public async Task<int> ExecuteAsync(Statement statement,
            CancellationToken cancellationToken = default) {
        await using var connectionProvider = _unitOfWorkManager.ConnectionProvider;
        await using var cmd = await CreateCommand(statement, connectionProvider, cancellationToken);
        var result = await cmd.ExecuteNonQueryAsync(cancellationToken);
        return result;
    }

    private async Task<DbCommand> CreateCommand(Statement statement,
            IConnectionProvider connectionProvider,
            CancellationToken cancellationToken = default) {
        var materializedStatement = statement.Materialize();
        var dbConnection = await connectionProvider
                .GetDbConnectionAsync(cancellationToken: cancellationToken);
        var cmd = dbConnection.CreateCommand();
        cmd.Connection = dbConnection;
        cmd.CommandText = materializedStatement.Sql;
        foreach (var queryParameter in materializedStatement.Parameters) {
            cmd.Parameters.Add(cmd.AddParameter(
                queryParameter.ParameterName, 
                queryParameter.Value,
                queryParameter.TypeCode
            ));
        }
        return cmd;
    }
}