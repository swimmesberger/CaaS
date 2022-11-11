using CaaS.Core.Base;

namespace CaaS.Core.CartAggregate; 

public interface ICartRepository : IRepository {
    Task<Cart?> FindCartByCustomerId(Guid customerId, CancellationToken cancellationToken = default);
    Task<Cart?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Cart> AddAsync(Cart entity, CancellationToken cancellationToken = default);
    Task<Cart> UpdateAsync(Cart entity, CancellationToken cancellationToken = default);
}