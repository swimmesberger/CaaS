using CaaS.Api.Middleware;

namespace CaaS.Api; 

public static class CaasAppBuilderExtensions {
    public static IApplicationBuilder UseCaas(this IApplicationBuilder app) {
        app.UseMiddleware<TenantMiddleware>();
        return app;
    }
}