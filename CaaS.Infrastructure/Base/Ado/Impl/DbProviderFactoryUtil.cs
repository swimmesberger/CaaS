using System.Data.Common;
using CaaS.Core.Base.Exceptions;
using CaaS.Infrastructure.Base.Ado.Model;
using Npgsql;

namespace CaaS.Infrastructure.Base.Ado.Impl; 

public static class DbProviderFactoryUtil {
    public const string PostgresProviderName = "Npgsql";
    // private const string SqlServerProviderName = "Microsoft.Data.SqlClient";
    
    public static DbProviderFactory GetDbProviderFactory(RelationalOptions connectionConfig) {
        // register all supported database factories
        // postgres
        DbProviderFactories.RegisterFactory(PostgresProviderName, NpgsqlFactory.Instance);
        // sql-server
        //DbProviderFactories.RegisterFactory(SqlServerProviderName, SqlClientFactory.Instance);
        return DbProviderFactories.GetFactory(connectionConfig.ProviderName);
    }

    public static CaasDbException ConvertException(DbException dbException) {
        if (dbException is not PostgresException postgresException) return new CaasDbException("Failed to execute statement", dbException);
        if (!postgresException.SqlState.Equals("23505")) return new CaasDbException("Failed to execute statement", dbException);
        return new CaasConstraintViolationDbException("Failed to execute statement because of constraint violation", postgresException) {
            TableName = postgresException.TableName,
            ConstraintName = postgresException.ConstraintName
        };
    }
}