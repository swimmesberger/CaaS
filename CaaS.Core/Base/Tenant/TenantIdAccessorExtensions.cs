using CaaS.Core.Base.Exceptions;

namespace CaaS.Core.Base.Tenant; 

public static class TenantIdAccessorExtensions {
    public static string GetTenantId(this ITenantIdAccessor tenantIdAccessor) {
        if (!tenantIdAccessor.TryGetTenantId(out var tenantId)) {
            throw new CaasNoTenantException("Can't determine tenant");
        }
        return tenantId;
    }
    
    public static Guid GetTenantGuid(this ITenantIdAccessor tenantIdAccessor) {
        return Guid.Parse(tenantIdAccessor.GetTenantId());
    }
}