using CaaS.Core.Exceptions;
using CaaS.Core.Repositories;
using CaaS.Core.Request;

namespace CaaS.Core.Tenant; 

public class ShopTenantService : ITenantService {
    private readonly IShopRepository _shopRepository;
    private readonly ITenantIdAccessor _tenantIdAccessor;
    
    private Tenant? _tenant; // request scoped cache

    public ShopTenantService(IShopRepository shopRepository, ITenantIdAccessor tenantIdAccessor) {
        _shopRepository = shopRepository;
        _tenantIdAccessor = tenantIdAccessor;
    }


    public async ValueTask<Tenant> GetTenantAsync(CancellationToken cancellationToken = default) {
        var tenant = _tenant ?? await GetTenantAsyncImpl(cancellationToken);
        _tenant = tenant;
        if (tenant == Tenant.Empty) {
            throw new CaasNoTenantException("Can't determine tenant");
        }
        return tenant;
    }

    public async ValueTask<Tenant?> GetTenantOrDefaultAsync(CancellationToken cancellationToken = default) {
        var tenant = _tenant ?? await GetTenantAsyncImpl(cancellationToken);
        _tenant = tenant;
        if (tenant == Tenant.Empty) {
            tenant = default;
        }
        return tenant;
    }

    public Task<IReadOnlyList<Tenant>> GetTenants(CancellationToken cancellationToken = default) {
        throw new NotImplementedException();
    }
    
    private async Task<Tenant> GetTenantAsyncImpl(CancellationToken cancellationToken = default) {
        if (!_tenantIdAccessor.TryGetTenant(out var tenantId)) {
            return Tenant.Empty;
        }
        var tenantGuid = Guid.Parse(tenantId);
        var shop = await _shopRepository.FindByIdAsync(tenantGuid, cancellationToken);
        var tenant = shop == null ? Tenant.Empty : new Tenant(tenantId, shop.Name);
        return tenant;
    }
}