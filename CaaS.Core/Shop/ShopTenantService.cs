using CaaS.Core.Base.Tenant;
using CaaS.Core.Exceptions;

namespace CaaS.Core.Shop; 

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

    public ValueTask<object> GetTenantIdAsync(CancellationToken cancellationToken = default)
        => new ValueTask<object>(GetTenantIdImpl());

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
        var tenantGuid = GetTenantIdImpl();
        var shop = await _shopRepository.FindByIdAsync(tenantGuid, cancellationToken);
        var tenant = shop == null ? Tenant.Empty : new Tenant(tenantGuid.ToString(), shop.Name);
        return tenant;
    }
    
    private Guid GetTenantIdImpl() {
        return _tenantIdAccessor.TryGetTenantId(out var tenantId) ? 
                Guid.Parse(tenantId) : Guid.Empty;
    }
}