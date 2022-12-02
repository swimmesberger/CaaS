using CaaS.Core.Base;
using CaaS.Core.ProductAggregate;

namespace CaaS.Core.CustomerAggregate; 

public interface ICustomerService {
    Task<Customer> AddCustomer(Customer customer, CancellationToken cancellationToken = default);
}