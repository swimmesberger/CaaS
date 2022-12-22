using CaaS.Core.CustomerAggregate;
using CaaS.Infrastructure.CustomerData;
using CaaS.Test.Common;

namespace CaaS.Test.RepositoryTests; 

public class CustomerRepositoryTest {
    private static readonly Guid CustomerIdA = new Guid("99C91EA1-4CA5-9097-DFDB-CF688F0DA31F");
    private static readonly Guid CustomerIdB = new Guid("EE6FD42A-FCF0-43A5-8C46-5339F6305388");
    private static readonly Guid TestShopId = new Guid("1AF5037B-16A0-430A-8035-6BCD785CBFB6");
    private static readonly string TestShopName = "TestShop";
    
    [Fact]
    public async Task UpdateCustomerEmail() {
        var customerId = new Guid("DD7B9FAD-19FD-4DFF-9E64-6D6C8A22C3FA");
        var customerDao = new MemoryDao<CustomerDataModel>(new List<CustomerDataModel>() {
            new CustomerDataModel() { Id = customerId, EMail = "test@example.com" }
        });
        var customerRepository = new CustomerRepository(customerDao);
        var customer = await customerRepository.FindByIdAsync(customerId);
        customer.Should().NotBeNull();
        customer = customer! with {
            EMail = "changed@example.com"
        };
        customer = await customerRepository.UpdateAsync(customer);
        customer.EMail.Should().Be("changed@example.com");
        
        customer = await customerRepository.FindByIdAsync(customerId);
        customer.Should().NotBeNull();
        customer!.EMail.Should().Be("changed@example.com");
    }

    [Fact]
    public async Task FindByEmailOptimistic() {
        var customerRepository = GetCustomerRepository();
        var customer = await customerRepository.FindByEmailAsync("simon@test.com");
        customer!.Id.Should().Be(CustomerIdB);

        customer = await customerRepository.FindByEmailAsync("ahahah@gmail.com");
        customer.Should().BeNull();
    }
    
    private ICustomerRepository GetCustomerRepository() {
        var customerDao = new MemoryDao<CustomerDataModel>(new List<CustomerDataModel>() {
            new CustomerDataModel { Id = CustomerIdA, ShopId = TestShopId, Name = "Roman Koho", EMail = "test@test.com", CreditCardNumber = "1111222233334444" },
            new CustomerDataModel { Id = CustomerIdB, ShopId = TestShopId, Name = "Simon Wimmes", EMail = "simon@test.com", CreditCardNumber = "9999999999999999" }
        });
        return new CustomerRepository(customerDao);
    }
}