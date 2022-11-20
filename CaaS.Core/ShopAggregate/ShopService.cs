using CaaS.Core.Base;
using CaaS.Core.Base.Exceptions;

namespace CaaS.Core.ShopAggregate; 

public class ShopService : IShopService {
    private readonly IShopRepository _shopRepository;
    private readonly IShopAdminRepository _shopAdminRepository;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public ShopService(IShopRepository shopRepository, IUnitOfWorkManager unitOfWorkManager, IShopAdminRepository shopAdminRepository) {
        _shopRepository = shopRepository;
        _unitOfWorkManager = unitOfWorkManager;
        _shopAdminRepository = shopAdminRepository;
    }
    
    public async Task<CountedResult<Shop>> GetAll(CancellationToken cancellationToken = default) {
        var items = await _shopRepository.FindAllAsync(cancellationToken);
        return new CountedResult<Shop>() { TotalCount = items.Count, Items = items };
    }
    
    public async Task<Shop?> GetByName(string name, CancellationToken cancellationToken = default) {
        return await _shopRepository.FindByNameAsync(name, cancellationToken);
    }
    
    public async Task<Shop> SetName(Guid id, string name, CancellationToken cancellationToken = default) {
        await using var uow = _unitOfWorkManager.Begin();
        var shop = await _shopRepository.FindByIdAsync(id, cancellationToken);
        if (shop == null) {
            throw new CaasItemNotFoundException();
        }
        shop = shop with { Name = name };
        shop = await _shopRepository.UpdateAsync(shop, cancellationToken);
        await uow.CompleteAsync(cancellationToken);
        return shop;
    }
    public async Task<Shop> Add(string name, Guid adminId, int cartLifetimeMinutes = 120, CancellationToken cancellationToken = default) {
        var admin = await _shopAdminRepository.FindByIdAsync(adminId, cancellationToken);
        if (admin == null) {
            throw new CaasItemNotFoundException();
        }
        var shop = new Shop {
            Name = name,
            CartLifetimeMinutes = cartLifetimeMinutes,
            ShopAdmin = admin
        };

        shop = await _shopRepository.AddAsync(shop, cancellationToken);
        
        return shop;
    }
}