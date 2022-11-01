using System.Diagnostics.CodeAnalysis;

namespace CaaS.Core.Request; 

public interface ITenantIdAccessor {
    bool TryGetTenantId([MaybeNullWhen(false)] out string tenantId);
}