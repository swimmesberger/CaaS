using CaaS.Core.Base;

namespace CaaS.Core.CustomerAggregate; 

public interface ICustomerRepository : IRepository {
    Task<Customer?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Customer>> FindByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);

    Task<Customer> UpdateAsync(Customer customer, CancellationToken cancellationToken = default);
}