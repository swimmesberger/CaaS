using CaaS.Core.Base.Exceptions;
using CaaS.Core.Base.Tenant;
using CaaS.Core.ShopAggregate;

namespace CaaS.Core.CustomerAggregate; 

public class CustomerService : ICustomerService {
    private readonly ITenantIdAccessor _tenantIdAccessor;
    private readonly ICustomerRepository _customerRepository;
    private readonly IShopRepository _shopRepository;

    public CustomerService(ITenantIdAccessor tenantIdAccessor, ICustomerRepository customerRepository, IShopRepository shopRepository) {
        _tenantIdAccessor = tenantIdAccessor;
        _customerRepository = customerRepository;
        _shopRepository = shopRepository;
    }
    public async Task<Customer> AddCustomer(Customer customer, CancellationToken cancellationToken = default) {
        var shop =  await _shopRepository.FindByIdAsync(_tenantIdAccessor.GetTenantGuid(), cancellationToken);
        if (shop == null) {
            throw new CaasItemNotFoundException();
        }
        
        customer = customer with{ ShopId = shop.Id };

        return await _customerRepository.AddAsync(customer, cancellationToken);
    }
}