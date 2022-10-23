using System.Data.Common;
using CaaS.Infrastructure.Repositories.Base.Mapping;

namespace CaaS.Infrastructure.Ado; 

public class AdoTemplate {
    private readonly IConnectionProvider _connectionProvider;

    public AdoTemplate(IConnectionProvider connectionProvider) {
        _connectionProvider = connectionProvider;
    }

    public async Task<List<T>> QueryAsync<T>(Statement statement,
            RowMapper<T> mapper,
            CancellationToken cancellationToken = default) {
        await using var cmd = await CreateCommand(statement, cancellationToken);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        var items = new List<T>();
        while (await reader.ReadAsync(cancellationToken)) {
            items.Add(mapper(reader));
        }
        return items;
    }

    public async Task<int> ExecuteAsync(Statement statement,
            CancellationToken cancellationToken = default) {
        await using var cmd = await CreateCommand(statement, cancellationToken);
        var result = await cmd.ExecuteNonQueryAsync(cancellationToken);
        return result;
    }

    private async Task<DbCommand> CreateCommand(Statement statement,
            CancellationToken cancellationToken = default) {
        var parameters = statement.Parameters;
        parameters ??= Enumerable.Empty<QueryParameter>();
        var dbConnection = await _connectionProvider
                .GetDbConnectionAsync(transactional: true, cancellationToken: cancellationToken);
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