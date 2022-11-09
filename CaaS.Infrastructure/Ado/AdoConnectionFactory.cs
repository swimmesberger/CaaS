using System.Data.Common;
using CaaS.Core.Exceptions;
using CaaS.Infrastructure.Ado.Base;
using CaaS.Infrastructure.Ado.Model;

namespace CaaS.Infrastructure.Ado; 

public class AdoConnectionFactory : IConnectionFactory {
    private readonly DbProviderFactory _dbProviderFactory;
    private readonly RelationalOptions _options;

    public AdoConnectionFactory(DbProviderFactory dbProviderFactory, RelationalOptions options) {
        _dbProviderFactory = dbProviderFactory;
        _options = options;
    }

    public DbConnection CreateConnection() {
        var dbConnection = _dbProviderFactory.CreateConnection();
        if (dbConnection == null) {
            throw new CaasDbException("Database Factory does not return a connection");
        }
        dbConnection.ConnectionString = _options.ConnectionString;
        return dbConnection;
    }

    public DbParameter CreateParameter() {
        var dbParameter = _dbProviderFactory.CreateParameter();
        if (dbParameter == null) {
            throw new CaasDbException("Database Factory does not return a parameter");
        }
        return dbParameter;
    }
}