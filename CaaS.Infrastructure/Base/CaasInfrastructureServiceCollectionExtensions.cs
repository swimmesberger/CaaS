using System.Data.Common;
using CaaS.Core.Base;
using CaaS.Core.Base.Tenant;
using CaaS.Core.BlobAggregate;
using CaaS.Core.CartAggregate;
using CaaS.Core.CouponAggregate;
using CaaS.Core.CustomerAggregate;
using CaaS.Core.OrderAggregate;
using CaaS.Core.DiscountAggregate.Base;
using CaaS.Core.ProductAggregate;
using CaaS.Core.ShopAggregate;
using CaaS.Infrastructure.Base.Ado;
using CaaS.Infrastructure.Base.Ado.Impl;
using CaaS.Infrastructure.Base.Ado.Impl.Npgsql;
using CaaS.Infrastructure.Base.Ado.Model;
using CaaS.Infrastructure.Base.Di;
using CaaS.Infrastructure.Base.Tenant;
using CaaS.Infrastructure.BlobData;
using CaaS.Infrastructure.CartData;
using CaaS.Infrastructure.CouponData;
using CaaS.Infrastructure.CustomerData;
using CaaS.Infrastructure.DiscountData;
using CaaS.Infrastructure.Gen;
using CaaS.Infrastructure.OrderData;
using CaaS.Infrastructure.ProductData;
using CaaS.Infrastructure.ShopData;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql.Logging;
using SwKo.Pay;
using SwKo.Pay.Api;

namespace CaaS.Infrastructure.Base; 

public static class CaasInfrastructureServiceCollectionExtensions {
    public static IServiceCollection AddCaasInfrastructure(this IServiceCollection services) {
        services.AddDatabase();
        services.AddRepositories();
        services.AddCaasDiscountInfrastructure();
        services.AddSingleton<IPaymentService, PaymentServiceImpl>();
        services.AddScoped<StaticTenantIdAccessor>();
        services.AddScoped<ITenantIdAccessor>(sp => sp.GetRequiredService<StaticTenantIdAccessor>());
        services.AddComposite<ITenantIdAccessor, CompositeTenantIdAccessor>();
        services.AddScoped<IAppKeyValidator, ShopAppKeyValidator>();
        services.AddScoped<IBlobService, BlobService>();
        return services;
    }

    public static IServiceCollection AddCaasDiscountInfrastructure(this IServiceCollection services) {
        services.AddOptions<DiscountJsonOptions>().Configure<IEnumerable<DiscountComponentMetadata>>((options, metadatas) 
            => options.JsonSerializerOptions.Converters.Add(new DiscountSettingsJsonConverter(metadatas)));
        services.AddScoped<IDiscountSettingRawConverter, DiscountSettingRawConverter>();
        return services;
    }
    
    public static void AddComposite<TInterface, TConcrete>(this IServiceCollection services)
        where TInterface : class
        where TConcrete : class, TInterface {
        var wrappedDescriptors = services.Where(s => s.ServiceType == typeof(TInterface)).ToList();
        foreach (var descriptor in wrappedDescriptors)
            services.Remove(descriptor);

        var objectFactory = ActivatorUtilities.CreateFactory(
            typeof(TConcrete),
            new[] { typeof(IEnumerable<TInterface>) });

        services.Add(ServiceDescriptor.Describe(
            typeof(TInterface),
            s => (TInterface)objectFactory(s, new object?[] { wrappedDescriptors.Select(s.CreateInstance).Cast<TInterface>() }),
            wrappedDescriptors.Select(d => d.Lifetime).Max())
        );
    }
    
    private static object CreateInstance(this IServiceProvider services, ServiceDescriptor descriptor) {
        if (descriptor.ImplementationInstance != null)
            return descriptor.ImplementationInstance;

        if (descriptor.ImplementationFactory != null)
            return descriptor.ImplementationFactory(services);

        return ActivatorUtilities.GetServiceOrCreateInstance(services, descriptor.ImplementationType ?? throw new ArgumentException("Invalid service descriptor"));
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
        services.AddScoped<IStatementMaterializer, AdoStatementMaterializer>();
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
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ICouponRepository, CouponRepository>();
        services.AddScoped<IShopAdminRepository, ShopAdminRepository>();
        services.AddScoped<IStatisticsService, StatisticsImpl>();
        services.AddScoped<IDiscountSettingRepository, DiscountSettingsRepository>();
        return services;
    }

    private static DbProviderFactory GetDbProviderFactory(IServiceProvider serviceProvider) {
        var connectionConfig = serviceProvider.GetRequiredService<RelationalOptions>();
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        if (connectionConfig.ProviderName.Equals(DbProviderFactoryUtil.PostgresProviderName)) {
            NpgsqlLogManager.Provider = new MicrosoftLoggingProvider(loggerFactory);
        }
        return DbProviderFactoryUtil.GetDbProviderFactory(connectionConfig);
    }
}