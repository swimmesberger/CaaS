using System.Data.Common;
using CaaS.Core.Repositories;
using CaaS.Core.Repositories.Base;
using CaaS.Core.Request;
using CaaS.Core.Tenant;
using CaaS.Generator.Common;
using CaaS.Infrastructure.Ado;
using CaaS.Infrastructure.Gen;
using CaaS.Infrastructure.Repositories;
using CaaS.Infrastructure.Repositories.Base.Mapping;
using Microsoft.Extensions.Options;
using Npgsql;

namespace CaaS.Api; 

public static class CaasServiceCollectionExtensions {
    private const string PostgresProviderName = "Npgsql";
    // private const string SqlServerProviderName = "Microsoft.Data.SqlClient";
    
    public static IServiceCollection AddConnectionFactory(this IServiceCollection services, IConfiguration configuration) {
        services.Configure<RelationalOptions>(configuration.GetSection(RelationalOptions.Key));
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<RelationalOptions>>().Value);
        services.AddSingleton(GetDbProviderFactory);
        services.AddSingleton<IConnectionFactory, ConnectionFactory>();
        return services;
    }

    private static DbProviderFactory GetDbProviderFactory(IServiceProvider serviceProvider) {
        var connectionConfig = serviceProvider.GetRequiredService<RelationalOptions>();
        // register all supported database factories
        // postgres
        DbProviderFactories.RegisterFactory(PostgresProviderName, NpgsqlFactory.Instance);
        // sql-server
        //DbProviderFactories.RegisterFactory(SqlServerProviderName, SqlClientFactory.Instance);
        return DbProviderFactories.GetFactory(connectionConfig.ProviderName);
    }

    public static IServiceCollection AddCaaS(this IServiceCollection services, IConfiguration configuration) {
        services.AddHttpContextAccessor();
        services.AddScoped<ITenantIdAccessor, HttpTenantIdAccessor>();
        services.AddScoped<IUnitOfWork, AdoUnitOfWork>();
        services.AddScoped<AdoUnitOfWorkManager>();
        services.AddScoped<IStatementExecutor, AdoTemplate>();
        services.AddScoped<IUnitOfWorkManager>(sp => sp.GetRequiredService<AdoUnitOfWorkManager>());
        services.AddScoped<IHasConnectionProvider>(sp => sp.GetRequiredService<AdoUnitOfWorkManager>());
        services.AddScoped<ITenantService, ShopTenantService>();
        services.AddScoped<IShopRepository, ShopRepository>();
        services.AddScoped(typeof(IStatementGenerator<>), typeof(StatementGenerator<>));
        services.AddTransient<PropertyNamingPolicy, PropertySnakeCaseNamingPolicy>();
        services.AddConnectionFactory(configuration);
        services.AddDataRecordMapper();
        return services;
    }
}