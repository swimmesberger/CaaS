using Microsoft.AspNetCore.Http.Features;

namespace CaaS.Api.Middleware; 

public class TenantMiddleware {
    private readonly RequestDelegate _next;
    
    public TenantMiddleware(RequestDelegate next) {
        _next = next;
    }

    public async Task Invoke(HttpContext context) {
        var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
        var attribute = endpoint?.Metadata.GetMetadata<RequireTenantAttribute>();
        if (attribute != null && !context.Request.Headers.ContainsKey(HeaderConstants.TenantId)) {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }            
        await _next(context); // Here the action in the controller is called
    }
}