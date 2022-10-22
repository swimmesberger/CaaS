using System.Data.Common;
using CaaS.Infrastructure.Repositories.Base.Mapping;

namespace CaaS.Infrastructure.Ado; 

public class AdoTemplate {
    private readonly IConnectionProvider _connectionProvider;

    public AdoTemplate(IConnectionProvider connectionProvider) {
        _connectionProvider = connectionProvider;
    }

    public async Task<List<T>> QueryAsync<T>(string sql, RowMapper<T> mapper, 
            IEnumerable<QueryParameter>? parameters = null, 
            CancellationToken cancellationToken = default) {
        await using var cmd = await CreateCommand(sql, parameters, cancellationToken);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        var items = new List<T>();
        while (await reader.ReadAsync(cancellationToken)) {
            items.Add(mapper(reader));
        }
        return items;
    }

    public async Task<int> ExecuteAsync(string sql, 
            IEnumerable<QueryParameter>? parameters = null,
            CancellationToken cancellationToken = default) {
        await using var cmd = await CreateCommand(sql, parameters, cancellationToken);
        var result = await cmd.ExecuteNonQueryAsync(cancellationToken);
        return result;
    }

    private async Task<DbCommand> CreateCommand(string sql, 
            IEnumerable<QueryParameter>? parameters = null,
            CancellationToken cancellationToken = default) {
        parameters ??= Enumerable.Empty<QueryParameter>();
        var dbConnection = await _connectionProvider
                .GetDbConnectionAsync(transactional: true, cancellationToken: cancellationToken);
        var cmd = dbConnection.CreateCommand();
        cmd.Connection = dbConnection;
        cmd.CommandText = sql;
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