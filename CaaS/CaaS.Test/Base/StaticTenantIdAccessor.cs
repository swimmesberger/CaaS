using CaaS.Core.Request;

namespace CaaS.Test.Integration; 

public class StaticTenantIdAccessor : ITenantIdAccessor {
    private readonly string _tenantId;

    public StaticTenantIdAccessor(string tenantId) {
        _tenantId = tenantId;
    }

    public bool TryGetTenantId(out string tenantId) {
        tenantId = _tenantId;
        return true;
    }
}