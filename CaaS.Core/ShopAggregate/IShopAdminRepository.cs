using CaaS.Core.Base;

namespace CaaS.Core.ShopAggregate;

public interface IShopAdminRepository : IRepository {
    Task<IReadOnlyList<ShopAdmin>> FindByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    Task<ShopAdmin?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ShopAdmin?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);
    
    Task<ShopAdmin> AddAsync(ShopAdmin entity, CancellationToken cancellationToken = default);
    Task<ShopAdmin> UpdateAsync(ShopAdmin oldEntity, ShopAdmin newEntity, CancellationToken cancellationToken = default);
    Task DeleteAsync(ShopAdmin entity, CancellationToken cancellationToken = default);
}