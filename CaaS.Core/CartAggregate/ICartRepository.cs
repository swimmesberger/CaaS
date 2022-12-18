using CaaS.Core.Base;

namespace CaaS.Core.CartAggregate; 

public interface ICartRepository : IRepository {
    Task<IReadOnlyList<Cart>> FindExpiredCarts(int lifeTimeMinutes, CancellationToken cancellationToken = default);
    Task<Cart?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<long> CountAsync(CancellationToken cancellationToken = default);
    Task<Cart> AddAsync(Cart entity, CancellationToken cancellationToken = default);
    Task<Cart> UpdateAsync(Cart oldEntity, Cart newEntity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Cart entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(IEnumerable<Cart> entities, CancellationToken cancellationToken = default);
}