using System.Data.Common;
using CaaS.Core.Base;
using CaaS.Core.Base.Tenant;
using CaaS.Core.Cart;
using CaaS.Core.Customer;
using CaaS.Core.Product;
using CaaS.Core.Shop;
using CaaS.Infrastructure.Base.Ado;
using CaaS.Infrastructure.Base.Ado.Impl;
using CaaS.Infrastructure.Base.Ado.Model;
using CaaS.Infrastructure.Base.Di;
using CaaS.Infrastructure.Cart;
using CaaS.Infrastructure.Customer;
using CaaS.Infrastructure.Gen;
using CaaS.Infrastructure.Product;
using CaaS.Infrastructure.Shop;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace CaaS.Infrastructure.Base; 

public static class CaasInfrastructureServiceCollectionExtensions {
    private const string PostgresProviderName = "Npgsql";
    // private const string SqlServerProviderName = "Microsoft.Data.SqlClient";
    
    public static IServiceCollection AddCaasInfrastructure(this IServiceCollection services) {
        services.AddDatabase();
        services.AddRepositories();
        return services;
    }

    // ReSharper disable once UnusedMethodReturnValue.Local
    private static IServiceCollection AddDatabase(this IServiceCollection services) {
        services.AddSingleton(GetDbProviderFactory);
        services.AddSingleton<IConnectionFactory, AdoConnectionFactory>();
        services.AddScoped<IUnitOfWork, AdoUnitOfWork>();
        services.AddScoped<AdoUnitOfWorkManager>();
        services.AddScoped<IStatementExecutor, AdoStatementExecutor>();
        services.AddScoped<IUnitOfWorkManager>(sp => sp.GetRequiredService<AdoUnitOfWorkManager>());
        services.AddScoped<IHasConnectionProvider>(sp => sp.GetRequiredService<AdoUnitOfWorkManager>());
        services.AddScoped(typeof(IDao<>), typeof(GenericDao<>));
        services.AddScoped(typeof(IStatementGenerator<>), typeof(AdoStatementGenerator<>));
        services.AddScoped(typeof(IServiceProvider<>), typeof(DefaultTypedServiceProvider<>));
        services.AddDataRecordMapper();
        return services;
    }
    
    // ReSharper disable once UnusedMethodReturnValue.Local
    private static IServiceCollection AddRepositories(this IServiceCollection services) {
        services.AddScoped<ITenantService, ShopTenantService>();
        services.AddScoped<IShopRepository, ShopRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ICartRepository, CartRepository>();

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
}