using CaaS.Core.Entities;
using CaaS.Core.Repositories.Base;

namespace CaaS.Core.Repositories; 

public interface ICartRepository : ICrudRepository<Cart> {
    Task<Cart?> FindCartByCustomerId(Guid customerId, CancellationToken cancellationToken = default);
    Task<Cart> UpdateAsync(Cart entity, CancellationToken cancellationToken = default);
}