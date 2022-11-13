using CaaS.Api.Base.Attributes;
using CaaS.Core.Base.Exceptions;
using Microsoft.AspNetCore.Http.Features;

namespace CaaS.Api.Base.Middleware; 

public class TenantMiddleware {
    private readonly RequestDelegate _next;
    
    public TenantMiddleware(RequestDelegate next) {
        _next = next;
    }

    public async Task Invoke(HttpContext context) {
        var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
        var attribute = endpoint?.Metadata.GetMetadata<RequireTenantAttribute>();
        if (attribute != null && !context.Request.Headers.ContainsKey(HeaderConstants.TenantId)) {
            throw new CaasNoTenantException();
        }
        await _next(context); // Here the action in the controller is called
    }
}