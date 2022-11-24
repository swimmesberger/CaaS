using System.Data.Common;
using CaaS.Core.Base;
using CaaS.Core.CartAggregate;
using CaaS.Core.CustomerAggregate;
using CaaS.Core.DiscountAggregate.Base;
using CaaS.Core.ProductAggregate;
using CaaS.Core.ShopAggregate;
using CaaS.Infrastructure.Base.Ado;
using CaaS.Infrastructure.Base.Ado.Impl;
using CaaS.Infrastructure.Base.Ado.Model;
using CaaS.Infrastructure.Base.Di;
using CaaS.Infrastructure.CartData;
using CaaS.Infrastructure.CustomerData;
using CaaS.Infrastructure.DiscountData;
using CaaS.Infrastructure.Gen;
using CaaS.Infrastructure.ProductData;
using CaaS.Infrastructure.ShopData;
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
        services.AddScoped<IShopRepository, ShopRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<IShopAdminRepository, ShopAdminRepository>();
        services.AddScoped<IDiscountSettingRepository, DiscountSettingsRepository>();
        services.AddOptions<DiscountJsonOptions>();
        services.AddScoped<DiscountSettingsJsonConverter>();
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