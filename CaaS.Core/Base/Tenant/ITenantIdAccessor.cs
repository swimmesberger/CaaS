using System.Diagnostics.CodeAnalysis;

namespace CaaS.Core.Base.Tenant; 

public interface ITenantIdAccessor {
    bool TryGetTenantId([MaybeNullWhen(false)] out string tenantId);
}