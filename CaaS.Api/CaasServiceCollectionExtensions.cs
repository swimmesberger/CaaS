using CaaS.Core.Request;
using CaaS.Infrastructure;
using CaaS.Infrastructure.Ado.Model;
using Microsoft.Extensions.Options;

namespace CaaS.Api; 

public static class CaasServiceCollectionExtensions {
    public static IServiceCollection AddCaas(this IServiceCollection services, IConfiguration configuration) {
        services.AddHttpContextAccessor();
        services.AddScoped<ITenantIdAccessor, HttpTenantIdAccessor>();
        services.Configure<RelationalOptions>(configuration.GetSection(RelationalOptions.Key));
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<RelationalOptions>>().Value);
        services.AddCaasInfrastructure();
        return services;
    }
}