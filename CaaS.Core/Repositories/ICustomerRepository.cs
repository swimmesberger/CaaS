using CaaS.Core.Entities;
using CaaS.Core.Repositories.Base;

namespace CaaS.Core.Repositories; 

public interface ICustomerRepository : IRepository {
    Task<Customer?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Customer>> FindByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
}