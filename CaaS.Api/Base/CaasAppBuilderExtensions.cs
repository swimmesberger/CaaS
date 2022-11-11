using CaaS.Api.Base.Middleware;

namespace CaaS.Api.Base; 

public static class CaasAppBuilderExtensions {
    public static IApplicationBuilder UseCaas(this IApplicationBuilder app) {
        app.UseMiddleware<TenantMiddleware>();
        return app;
    }
}