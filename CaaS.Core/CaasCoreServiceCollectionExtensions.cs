using CaaS.Core.CartAggregate;
using CaaS.Core.ProductAggregate;
using CaaS.Core.ShopAggregate;
using Microsoft.Extensions.DependencyInjection;

namespace CaaS.Core; 

public static class CaasCoreServiceCollectionExtensions {
    public static IServiceCollection AddCaasCore(this IServiceCollection services) {
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IShopService, ShopService>();
        return services;
    }
}