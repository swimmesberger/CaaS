using CaaS.Core.Base;

namespace CaaS.Core.ShopAggregate; 

public interface IShopService {
    Task<CountedResult<Shop>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Shop?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Shop?> GetByAdminIdAsync(Guid adminId, CancellationToken cancellationToken = default);
    Task<Shop?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Shop> SetNameAsync(Guid id, string name, CancellationToken cancellationToken = default);
    Task<Shop> AddAsync(Shop shop, CancellationToken cancellationToken = default);
    Task<Shop> UpdateAsync(Guid id, Shop updatedShop, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}