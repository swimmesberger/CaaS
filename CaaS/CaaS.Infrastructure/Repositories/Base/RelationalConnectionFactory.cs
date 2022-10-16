using System.Data;
using System.Data.Common;
using CaaS.Core.Exceptions;
using CaaS.Core.Repositories.Base;

namespace CaaS.Infrastructure.Repositories.Base; 

public class RelationalConnectionFactory : IRelationalConnectionFactory {
    private readonly DbProviderFactory _dbProviderFactory;
    private readonly RelationalOptions _options;

    public RelationalConnectionFactory(DbProviderFactory dbProviderFactory, RelationalOptions options) {
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
    public DbParameter CreateParameter(string name, DbType type) {
        var parameter = _dbProviderFactory.CreateParameter();
        if (parameter == null) {
            throw new CaasDbException("Database Factory does not return a parameter");
        }
        parameter.ParameterName = name;
        parameter.DbType = type;
        return parameter;
    }
}