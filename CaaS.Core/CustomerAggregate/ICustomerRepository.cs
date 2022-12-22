using CaaS.Core.Base;

namespace CaaS.Core.CustomerAggregate; 

public interface ICustomerRepository : IRepository {
    Task<Customer?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Customer?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Customer>> FindByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);

    Task<Customer> AddAsync(Customer entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Customer entity, CancellationToken cancellationToken = default);
    Task<Customer> UpdateAsync(Customer oldEntity, Customer newEntity, CancellationToken cancellationToken = default);
}