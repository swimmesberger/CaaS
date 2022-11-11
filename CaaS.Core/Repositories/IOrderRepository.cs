using System.Collections.Immutable;
using CaaS.Core.Entities;
using CaaS.Core.Repositories.Base;

namespace CaaS.Core.Repositories; 

public interface IOrderRepository : ICrudRepository<Order>, ICrudBulkWriteRepository<Order> {
    Task<IReadOnlyList<Order>> FindByCustomerId(Guid customerId, CancellationToken cancellationToken = default);
}