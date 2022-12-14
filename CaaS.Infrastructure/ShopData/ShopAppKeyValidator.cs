using CaaS.Core.Base.Tenant;
using CaaS.Core.ShopAggregate;
using CaaS.Infrastructure.Base;

namespace CaaS.Infrastructure.ShopData; 

public class ShopAppKeyValidator : IAppKeyValidator {
    private readonly IShopRepository _shopRepository;
    private readonly ITenantIdAccessor _tenantIdAccessor;
    
    public ShopAppKeyValidator(IShopRepository shopRepository, ITenantIdAccessor tenantIdAccessor) {
        _shopRepository = shopRepository;
        _tenantIdAccessor = tenantIdAccessor;
    }
    
    public async Task<bool> ValidateAppKeyAsync(string appKey, CancellationToken cancellationToken = default)
        => await _shopRepository.VerifyAppKeyAsync(_tenantIdAccessor.GetTenantGuid(), appKey,cancellationToken);
}