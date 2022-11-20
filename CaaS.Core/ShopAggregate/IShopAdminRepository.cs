using CaaS.Core.Base;

namespace CaaS.Core.ShopAggregate;

public interface IShopAdminRepository : IRepository {
    Task<IReadOnlyList<ShopAdmin>> FindByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    Task<ShopAdmin?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
}