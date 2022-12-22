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
    
    public async Task<CountedResult<Shop>> GetAllAsync(CancellationToken cancellationToken = default) {
        var shops = await _shopRepository.FindAllAsync(cancellationToken);
        return new CountedResult<Shop>() { Items = shops, TotalCount = shops.Count };
    }
    
    public async Task<Shop?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) {
        return await _shopRepository.FindByIdAsync(id, cancellationToken);
    }
    
    public async Task<Shop?> GetByAdminIdAsync(Guid adminId, CancellationToken cancellationToken = default) {
        return await _shopRepository.FindByAdminIdAsync(adminId, cancellationToken);
    }

    public async Task<Shop?> GetByNameAsync(string name, CancellationToken cancellationToken = default) {
        return await _shopRepository.FindByNameAsync(name, cancellationToken);
    }
    
    public async Task<Shop> SetNameAsync(Guid id, string name, CancellationToken cancellationToken = default) {
        await using var uow = _unitOfWorkManager.Begin();
        var shop = await _shopRepository.FindByIdAsync(id, cancellationToken);
        if (shop == null) {
            throw new CaasItemNotFoundException($"Shop {id} not found");
        }
        var updatedShop = shop with { Name = name };
        shop = await _shopRepository.UpdateAsync(shop, updatedShop, cancellationToken);
        await uow.CompleteAsync(cancellationToken);
        return shop;
    }
    public async Task<Shop> AddAsync(Shop shop, CancellationToken cancellationToken = default) {
        var admin = await _shopAdminRepository.FindByIdAsync(shop.ShopAdmin.Id, cancellationToken);
        if (admin == null) {
            throw new CaasItemNotFoundException($"ShopAdmin {shop.ShopAdmin.Id} does not exist");
        }

        shop = shop with {
            ShopAdmin = admin
        };
        
        shop = await _shopRepository.AddAsync(shop, cancellationToken);
        
        return shop;
    }
    public async Task<Shop> UpdateAsync(Guid id, Shop updatedShop, CancellationToken cancellationToken = default) {
        var oldShop = await _shopRepository.FindByIdAsync(id, cancellationToken);
        if (oldShop == null) {
            throw new CaasItemNotFoundException($"Shop '{id}' not found");
        }

        var shopAdmin = await _shopAdminRepository.FindByIdAsync(updatedShop.ShopAdmin.Id, cancellationToken);
        if (shopAdmin == null) {
            throw new CaasItemNotFoundException($"ShopAdmin '{updatedShop.ShopAdmin.Id}' not found");
        }
        
        updatedShop = updatedShop with {
            Id = id,
            ShopAdmin = shopAdmin
        };

        return await _shopRepository.UpdateAsync(oldShop, updatedShop, cancellationToken);
    }
    
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default) {
        var shop = await _shopRepository.FindByIdAsync(id, cancellationToken);
        if (shop == null) {
            throw new CaasItemNotFoundException($"ShopId {id} not found");
        }
        
        await _shopRepository.DeleteAsync(shop, cancellationToken);
    }
}