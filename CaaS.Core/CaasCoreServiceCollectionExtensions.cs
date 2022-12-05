using CaaS.Core.CartAggregate;
using CaaS.Core.OrderAggregate;
using CaaS.Core.Base;
using CaaS.Core.CouponAggregate;
using CaaS.Core.CustomerAggregate;
using CaaS.Core.DiscountAggregate;
using CaaS.Core.DiscountAggregate.Base;
using CaaS.Core.DiscountAggregate.Impl;
using CaaS.Core.ProductAggregate;
using CaaS.Core.ShopAggregate;
using Microsoft.Extensions.DependencyInjection;

namespace CaaS.Core; 

public static class CaasCoreServiceCollectionExtensions {
    public static IServiceCollection AddCaasCore(this IServiceCollection services) {
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IShopService, ShopService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IDateTimeOffsetProvider, DefaultDateTimeOffsetProvider>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<ICouponService, CouponService>();

        services = services.AddCaasDiscountCore();
        return services;
    }

    public static IServiceCollection AddCaasDiscountCore(this IServiceCollection services) {
        services.AddSingleton<IDiscountComponentFactory, DiscountComponentFactory>();
        services.AddSingleton<IDiscountComponentProvider>(sp => sp.GetRequiredService<IDiscountComponentFactory>());
        services.AddScoped<IDiscountService, CaasDiscountService>();
        services.AddDiscountRule<MinProductCountDiscountRule, MinProductCountSettings>(MinProductCountDiscountRule.Id);
        services.AddDiscountRule<TimeWindowDiscountRule, TimeWindowDiscountSettings>(TimeWindowDiscountRule.Id);
        services.AddDiscountRule<CompositeDiscountRule, CompositeDiscountRuleSettings>(CompositeDiscountRule.Id);
        services.AddDiscountAction<PercentageDiscountAction, PercentageDiscountSettings>(PercentageDiscountAction.Id);
        services.AddDiscountAction<FixedValueDiscountAction, FixedValueDiscountSettings>(FixedValueDiscountAction.Id);
        services.AddDiscountAction<AndDiscountAction, AndDiscountActionSettings>(AndDiscountAction.Id);
        return services;
    }
}