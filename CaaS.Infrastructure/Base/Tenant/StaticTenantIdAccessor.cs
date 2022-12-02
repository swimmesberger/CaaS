using System.Diagnostics.CodeAnalysis;
using CaaS.Core.Base.Tenant;
using Microsoft.Extensions.DependencyInjection;

namespace CaaS.Infrastructure.Base.Tenant; 

public sealed class StaticTenantIdAccessor : ITenantIdAccessor {
    public string? TenantId { get; set; }
    public int Priority => 10;

    [ActivatorUtilitiesConstructor]
    // ReSharper disable once UnusedMember.Global
    public StaticTenantIdAccessor() { }
    
    public StaticTenantIdAccessor(string tenantId) {
        TenantId = tenantId;
    }

    public void SetTenantGuid(Guid id) => TenantId = id.ToString();

    public bool TryGetTenantId([MaybeNullWhen(false)] out string tenantId) {
        if (TenantId != null) {
            tenantId = TenantId;
            return true;
        }
        tenantId = default;
        return false;
    }
}