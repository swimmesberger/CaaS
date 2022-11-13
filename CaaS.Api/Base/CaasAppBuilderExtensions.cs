using CaaS.Api.Base.Middleware;
using Hellang.Middleware.ProblemDetails;

namespace CaaS.Api.Base; 

public static class CaasAppBuilderExtensions {
    public static IApplicationBuilder UseCaas(this IApplicationBuilder app) {
        app.UseProblemDetails();
        app.UseMiddleware<TenantMiddleware>();
        return app;
    }
}