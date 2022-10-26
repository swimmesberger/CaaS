using System.Data.Common;

namespace CaaS.Infrastructure.Ado; 

public class AdoTemplate : IStatementExecutor {
    private readonly IHasConnectionProvider _unitOfWorkManager;

    public AdoTemplate(IHasConnectionProvider unitOfWorkManager) {
        _unitOfWorkManager = unitOfWorkManager;
    }

    public async Task<object?> QueryScalarAsync(Statement statement, CancellationToken cancellationToken = default) {
        await using var connectionProvider = _unitOfWorkManager.ConnectionProvider;
        await using var cmd = await CreateCommand(statement, connectionProvider, transactional: false, cancellationToken);
        return await cmd.ExecuteScalarAsync(cancellationToken);
    }

    public async Task<List<T>> QueryAsync<T>(Statement statement,
            RowMapper<T> mapper,
            CancellationToken cancellationToken = default) {
        await using var connectionProvider = _unitOfWorkManager.ConnectionProvider;
        await using var cmd = await CreateCommand(statement, connectionProvider, transactional: false, cancellationToken);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        var items = new List<T>();
        while (await reader.ReadAsync(cancellationToken)) {
            items.Add(mapper(reader));
        }
        return items;
    }

    public async Task<int> ExecuteAsync(Statement statement,
            CancellationToken cancellationToken = default) {
        await using var connectionProvider = _unitOfWorkManager.ConnectionProvider;
        await using var cmd = await CreateCommand(statement, connectionProvider, transactional: true, cancellationToken);
        var result = await cmd.ExecuteNonQueryAsync(cancellationToken);
        return result;
    }

    private async Task<DbCommand> CreateCommand(Statement statement,
            IConnectionProvider connectionProvider,
            bool transactional,
            CancellationToken cancellationToken = default) {
        var parameters = statement.Parameters;
        parameters ??= Enumerable.Empty<QueryParameter>();
        var dbConnection = await connectionProvider
                .GetDbConnectionAsync(transactional, cancellationToken: cancellationToken);
        var cmd = dbConnection.CreateCommand();
        cmd.Connection = dbConnection;
        cmd.CommandText = statement.Sql;
        foreach (var queryParameter in parameters) {
            cmd.Parameters.Add(cmd.AddParameter(
                    queryParameter.Name, 
                    queryParameter.Value,
                    queryParameter.TypeCode
            ));
        }
        return cmd;
    }
}