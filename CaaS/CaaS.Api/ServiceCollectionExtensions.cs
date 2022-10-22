using System.Data.Common;
using CaaS.Core.Repositories;
using CaaS.Core.Repositories.Base;
using CaaS.Core.Request;
using CaaS.Core.Tenant;
using CaaS.Infrastructure.Ado;
using CaaS.Infrastructure.Repositories;
using Microsoft.Extensions.Options;
using Npgsql;

namespace CaaS.Api; 

public static class ServiceCollectionExtensions {
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
        services.AddScoped<IRequestDataAccessor, HttpRequestDataAccessor>();
        services.AddScoped<AdoUnitOfWork>();
        services.AddScoped<IConnectionProvider>(sp => sp.GetRequiredService<AdoUnitOfWork>());
        services.AddScoped<IUnitOfWork>(static sp => {
            var uow = sp.GetRequiredService<AdoUnitOfWork>();
            uow.Implicit = false;
            return uow;
        });
        services.AddScoped<IUnitOfWorkManager, AdoUnitOfWorkManager>();
        services.AddScoped<ITenantService, ShopTenantService>();
        services.AddScoped<IShopRepository, ShopRepository>();
        services.AddConnectionFactory(configuration);
        return services;
    }
}