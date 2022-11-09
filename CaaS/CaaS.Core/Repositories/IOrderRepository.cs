using System.Collections.Immutable;
using CaaS.Core.Entities;
using CaaS.Core.Repositories.Base;

namespace CaaS.Core.Repositories; 

public interface IOrderRepository : ICrudRepository<Order> {
    Task<IReadOnlyList<Order>> FindOrdersByCustomerId(Guid customerId, CancellationToken cancellationToken = default);
}