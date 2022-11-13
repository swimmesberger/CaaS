using CaaS.Core.Base;
using CaaS.Core.Base.Exceptions;

namespace CaaS.Core.ShopAggregate; 

public class ShopService : IShopService {
    private readonly IShopRepository _shopRepository;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public ShopService(IShopRepository shopRepository, IUnitOfWorkManager unitOfWorkManager) {
        _shopRepository = shopRepository;
        _unitOfWorkManager = unitOfWorkManager;
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
}