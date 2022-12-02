using System.Diagnostics.CodeAnalysis;

namespace CaaS.Core.Base.Tenant; 

public interface ITenantIdAccessor {
    int Priority => 0;
    
    bool TryGetTenantId([MaybeNullWhen(false)] out string tenantId);
}