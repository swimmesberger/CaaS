using System.Diagnostics.CodeAnalysis;
using CaaS.Core.Base.Tenant;

namespace CaaS.Infrastructure.Base.Tenant; 

public sealed class CompositeTenantIdAccessor : ITenantIdAccessor {
    private readonly IEnumerable<ITenantIdAccessor> _tenantIdAccessors;

    public CompositeTenantIdAccessor(IEnumerable<ITenantIdAccessor> tenantIdAccessors) {
        _tenantIdAccessors = tenantIdAccessors.OrderByDescending(t => t.Priority);
    }
    
    public CompositeTenantIdAccessor() : this(Enumerable.Empty<ITenantIdAccessor>()) { }

    public bool TryGetTenantId([MaybeNullWhen(false)] out string tenantId) {
        foreach (var tenantIdAccessor in _tenantIdAccessors) {
            if (tenantIdAccessor.TryGetTenantId(out tenantId)) {
                return true;
            }
        }
        tenantId = default;
        return false;
    }
}