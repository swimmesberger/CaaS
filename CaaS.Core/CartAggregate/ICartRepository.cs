using CaaS.Core.Base;

namespace CaaS.Core.CartAggregate; 

public interface ICartRepository : IRepository {
    Task<Cart?> FindCartByCustomerId(Guid customerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Cart>> FindCartsByShopId(Guid shopId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Cart>> FindExpiredCarts(int lifeTimeMinutes, CancellationToken cancellationToken = default);
    Task<Cart?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Cart> AddAsync(Cart entity, CancellationToken cancellationToken = default);
    Task<Cart> UpdateAsync(Cart entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Cart entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(IEnumerable<Cart> entities, CancellationToken cancellationToken = default);
}