using CaaS.Core.Base;

namespace CaaS.Core.Customer; 

public interface ICustomerRepository : IRepository {
    Task<Entities.Customer?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Entities.Customer>> FindByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);

    Task<Entities.Customer> UpdateAsync(Entities.Customer customer, CancellationToken cancellationToken = default);
}