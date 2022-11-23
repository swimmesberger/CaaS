using CaaS.Core.DiscountAggregate.Models;
using Microsoft.Extensions.DependencyInjection;

namespace CaaS.Core.DiscountAggregate.Base; 

public static class DiscountServiceCollectionExtensions {
    public static IServiceCollection AddDiscountRule<TService, TSetting>(this IServiceCollection services, Guid id) 
        where TService : IDiscountRule 
        where TSetting: DiscountParameters {
        services.AddSingleton(_ => 
                new DiscountComponentMetadata(id, typeof(TService), typeof(TSetting), DiscountComponentType.Rule));
        return services;
    }
    
    public static IServiceCollection AddDiscountAction<TService, TSetting>(this IServiceCollection services, Guid id) 
        where TService : IDiscountAction
        where TSetting: DiscountParameters {
        services.AddSingleton(_ => 
                new DiscountComponentMetadata(id, typeof(TService), typeof(TSetting), DiscountComponentType.Action));
        return services;
    }
}