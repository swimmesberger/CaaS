using CaaS.Infrastructure.CustomerData;
using CaaS.Test.Common;

namespace CaaS.Test.RepositoryTests; 

public class CustomerRepositoryTest {
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
}