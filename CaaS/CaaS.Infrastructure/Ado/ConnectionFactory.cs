using System.Data.Common;
using CaaS.Core.Exceptions;

namespace CaaS.Infrastructure.Ado; 

public class ConnectionFactory : IConnectionFactory {
    private readonly DbProviderFactory _dbProviderFactory;
    private readonly RelationalOptions _options;

    public ConnectionFactory(DbProviderFactory dbProviderFactory, RelationalOptions options) {
        _dbProviderFactory = dbProviderFactory;
        _options = options;
    }

    public DbConnection CreateDbConnection() {
        var dbConnection = _dbProviderFactory.CreateConnection();
        if (dbConnection == null) {
            throw new CaasDbException("Database Factory does not return a connection");
        }
        dbConnection.ConnectionString = _options.ConnectionString;
        return dbConnection;
    }
}