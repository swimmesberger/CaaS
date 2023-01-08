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
        await using var uow = _unitOfWorkManager.Begin();
        var shopAdmin = await _shopAdminRepository.FindByIdAsync(shop.ShopAdmin.Id, cancellationToken);
        if (shopAdmin == null) {
            if (shop.ShopAdmin.Id == Guid.Empty) {
                throw new CaasItemNotFoundException($"ShopAdmin '{shop.ShopAdmin.Id}' not found");
            }
            shopAdmin = shop.ShopAdmin with { ShopId = shop.Id };
            try {
                shopAdmin = await _shopAdminRepository.AddAsync(shopAdmin, cancellationToken);
            } catch (CaasConstraintViolationDbException ex) {
                throw new CaasDuplicateCustomerEmailException("Duplicate shop admin e-mail") {
                    Type = ex.ConstraintName
                };
            }
        }
        shop = shop with {
            ShopAdmin = shopAdmin
        };
        shop = await _shopRepository.AddAsync(shop, cancellationToken);
        await uow.CompleteAsync(cancellationToken);
        return shop;
    }
    
    public async Task<Shop> UpdateAsync(Shop updatedShop, CancellationToken cancellationToken = default) {
        await using var uow = _unitOfWorkManager.Begin();
        var oldShop = await _shopRepository.FindByIdAsync(updatedShop.Id, cancellationToken);
        if (oldShop == null) {
            throw new CaasItemNotFoundException($"Shop '{updatedShop.Id}' not found");
        }

        if (updatedShop.ShopAdmin.Id != Guid.Empty) {
            var fShopAdmin = await _shopAdminRepository.FindByIdAsync(updatedShop.ShopAdmin.Id, cancellationToken);
            var shopAdmin = fShopAdmin ?? throw new CaasItemNotFoundException($"ShopAdmin '{updatedShop.ShopAdmin.Id}' not found");
            var updatedShopAdmin = shopAdmin with {
                Name = updatedShop.ShopAdmin.Name,
                EMail = updatedShop.ShopAdmin.EMail
            };
            updatedShopAdmin = await _shopAdminRepository.UpdateAsync(shopAdmin, updatedShopAdmin, cancellationToken);
            updatedShop = updatedShop with {
                ShopAdmin = updatedShopAdmin
            };
        }
        updatedShop = await _shopRepository.UpdateAsync(oldShop, updatedShop, cancellationToken);
        await uow.CompleteAsync(cancellationToken);
        return updatedShop;
    }
    
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default) {
        await using var uow = _unitOfWorkManager.Begin();
        var shop = await _shopRepository.FindByIdAsync(id, cancellationToken);
        if (shop == null) {
            throw new CaasItemNotFoundException($"ShopId {id} not found");
        }
        await _shopRepository.DeleteAsync(shop, cancellationToken);
        if (shop.ShopAdmin.Id != Guid.Empty) {
            await _shopAdminRepository.DeleteAsync(shop.ShopAdmin, cancellationToken);
        }
        await uow.CompleteAsync(cancellationToken);
    }
}