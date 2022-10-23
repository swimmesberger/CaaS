using System.Diagnostics.CodeAnalysis;

namespace CaaS.Core.Request; 

public interface ITenantIdAccessor {
    bool TryGetTenant([MaybeNullWhen(false)] out string tenantId);
}