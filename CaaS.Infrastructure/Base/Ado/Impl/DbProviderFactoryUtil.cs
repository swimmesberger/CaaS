using System.Data.Common;
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
}