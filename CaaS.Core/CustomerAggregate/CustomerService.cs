using CaaS.Core.Base;
using CaaS.Core.Base.Exceptions;
using CaaS.Core.Base.Tenant;
using CaaS.Core.ShopAggregate;

namespace CaaS.Core.CustomerAggregate; 

public class CustomerService : ICustomerService {
    private readonly ITenantIdAccessor _tenantIdAccessor;
    private readonly ICustomerRepository _customerRepository;
    private readonly IShopRepository _shopRepository;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public CustomerService(ITenantIdAccessor tenantIdAccessor, ICustomerRepository customerRepository, IShopRepository shopRepository, 
        IUnitOfWorkManager unitOfWorkManager) {
        _tenantIdAccessor = tenantIdAccessor;
        _customerRepository = customerRepository;
        _shopRepository = shopRepository;
        _unitOfWorkManager = unitOfWorkManager;
    }
    
    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) {
        return await _customerRepository.FindByIdAsync(id, cancellationToken);
    }
    public async Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) {
        return await _customerRepository.FindByEmailAsync(email, cancellationToken);
    }

    public async Task<Customer> AddAsync(Customer customer, CancellationToken cancellationToken = default) {
        await using var uow = _unitOfWorkManager.Begin();
        var shop =  await _shopRepository.FindByIdAsync(_tenantIdAccessor.GetTenantGuid(), cancellationToken);
        if (shop == null) {
            throw new CaasNoTenantException();
        }
        
        customer = customer with{ ShopId = shop.Id };
        customer = await _customerRepository.AddAsync(customer, cancellationToken);
        await uow.CompleteAsync(cancellationToken);
        return customer;
    }
    
    public async Task<Customer> UpdateAsync(Guid id, Customer updatedCustomer, CancellationToken cancellationToken = default) {
        await using var uow = _unitOfWorkManager.Begin();
        var oldCustomer = await _customerRepository.FindByIdAsync(id, cancellationToken);
        if (oldCustomer == null) {
            throw new CaasItemNotFoundException($"CustomerId {id} not found");
        }

        updatedCustomer = updatedCustomer with {
            Id = id,
            ShopId = _tenantIdAccessor.GetTenantGuid()
        };
        
        updatedCustomer = await _customerRepository.UpdateAsync(oldCustomer, updatedCustomer, cancellationToken);
        await uow.CompleteAsync(cancellationToken);
        return updatedCustomer;
    }
    
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default) {
        await using var uow = _unitOfWorkManager.Begin();
        var customer = await _customerRepository.FindByIdAsync(id, cancellationToken);
        if (customer == null) {
            throw new CaasItemNotFoundException($"CouponId {id} not found");
        }

        await _customerRepository.DeleteAsync(customer, cancellationToken);
        await uow.CompleteAsync(cancellationToken);
    }
}