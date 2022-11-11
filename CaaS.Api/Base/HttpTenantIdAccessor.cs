using System.Diagnostics.CodeAnalysis;
using CaaS.Core.Base.Tenant;

namespace CaaS.Api.Base; 

public class HttpTenantIdAccessor : ITenantIdAccessor {
    private readonly HttpContext? _httpContext;

    public HttpTenantIdAccessor(IHttpContextAccessor contextAccessor) {
        _httpContext = contextAccessor.HttpContext;
    }

    public bool TryGetTenantId([MaybeNullWhen(false)] out string tenantId) {
        if (_httpContext == null) {
            tenantId = default;
            return false;
        }
        if (!_httpContext.Request.Headers.TryGetValue(HeaderConstants.TenantId, out var tenantIdVals)) {
            tenantId = default;
            return false;
        }
        var opTenantId = tenantIdVals.FirstOrDefault();
        if (opTenantId == null) {
            tenantId = default;
            return false;
        }
        tenantId = opTenantId;
        return true;
    }
}