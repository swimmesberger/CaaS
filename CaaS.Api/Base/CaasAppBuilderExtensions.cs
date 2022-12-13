using CaaS.Api.Base.Middleware;
using Hellang.Middleware.ProblemDetails;

namespace CaaS.Api.Base; 

public static class CaasAppBuilderExtensions {
    public static IApplicationBuilder UseCaas(this IApplicationBuilder app) {
        var environment = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
        // Configure the HTTP request pipeline.
        if (environment.IsDevelopment()) {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseHttpLogging();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseProblemDetails();
        app.UseMiddleware<TenantMiddleware>();
        if (app is IEndpointRouteBuilder endpoint) {
            endpoint.MapControllers();
        }
        return app;
    }
}