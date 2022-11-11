using CaaS.Core.Base;

namespace CaaS.Core.Cart; 

public interface ICartRepository : IRepository {
    Task<Entities.Cart?> FindCartByCustomerId(Guid customerId, CancellationToken cancellationToken = default);
    Task<Entities.Cart?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Entities.Cart> AddAsync(Entities.Cart entity, CancellationToken cancellationToken = default);
    Task<Entities.Cart> UpdateAsync(Entities.Cart entity, CancellationToken cancellationToken = default);
}