using CaaS.Core.Entities;
using CaaS.Core.Exceptions;
using CaaS.Infrastructure.Ado.Base;
using CaaS.Infrastructure.Ado.Model;
using CaaS.Infrastructure.DataModel;
using CaaS.Infrastructure.Gen;

namespace CaaS.Test.Integration; 

public class CustomerDaoTest : BaseDaoTest {
    private const string ShopTenantId = "a468d796-db09-496d-9794-f6b42f8c7c0b";
    
    [Fact]
    public async Task FindAllWhenDbHasEntries() {
        var customerDao = GetCustomerDao(ShopTenantId);
        var customers = await customerDao.FindAllAsync().ToListAsync();
        customers[0].Id.Should().Be("9234a988-0abd-4b44-808a-9e7a8852e19c");
        customers[1].Id.Should().Be("703111b0-c3fd-4bf1-9d1a-12cd3852c182");
    }
    
    [Fact]
    public async Task FindByIdsWhenIdsAreValidReturnsEntities() {
        var customerDao = GetCustomerDao(ShopTenantId);
        var idList = new List<Guid> {
                Guid.Parse("9234a988-0abd-4b44-808a-9e7a8852e19c"),
                Guid.Parse("703111b0-c3fd-4bf1-9d1a-12cd3852c182")
        };
        
        var customers = await customerDao.FindByIdsAsync(idList).ToListAsync();
        
        customers[0].Id.Should().Be("9234a988-0abd-4b44-808a-9e7a8852e19c");
        customers[1].Id.Should().Be("703111b0-c3fd-4bf1-9d1a-12cd3852c182");
    }
    
    [Fact]
    public async Task FindByNameAndEMailReturnsEntities() {
        var customerDao = GetCustomerDao(ShopTenantId);
        
        var parameters = new List<QueryParameter> {
                new(nameof(Customer.Name), "Frances Hallums"),
                new(nameof(Customer.EMail), "fhallums2r@edublogs.org"),
        };

        var customers = await customerDao.FindBy(StatementParameters
                .CreateWhere(parameters)).ToListAsync();
        
        customers.Count.Should().NotBe(0);
        customers[0].Id.Should().Be("9234a988-0abd-4b44-808a-9e7a8852e19c");
    }
    
    [Fact]
    public async Task CountReturnsNumberOfEntitiesInDb() {
        var customerDao = GetCustomerDao(ShopTenantId);
        (await customerDao.CountAsync()).Should().Be(3);
    }
    
    [Fact]
    public async Task AddAddsNewEntityToDb() {
        var customerDao = GetCustomerDao(ShopTenantId);
        var customer = new CustomerDataModel() {
                Id = Guid.Parse("7A819343-23A1-4AD9-8798-64D1047CF01F"),
                ShopId = Guid.Parse(ShopTenantId),
                Name = "Roman Kofler-Hofer",
                EMail = "test@email.com",
                CreditCardNumber = "1111222233334444"
        };
        await customerDao.AddAsync(customer);
        
        customer = await customerDao.FindByIdAsync(customer.Id);
        customer.Should().NotBeNull();
        customer!.Id.Should().Be("7A819343-23A1-4AD9-8798-64D1047CF01F");
    }

    [Fact]
    public async Task UpdateEntityWhenEntityIsExisting() {
        var customerId = Guid.Parse("c63b840a-a520-4a6a-a5d1-7328618c20c5");
        var customerDao = GetCustomerDao(ShopTenantId);
        var customer = await customerDao.FindByIdAsync(customerId);
        customer.Should().NotBeNull();
        customer!.Name.Should().Be("Alex Cossentine");
        customer = customer with {
                Name = "Alex Cossentine-Kofler"
        };
        await customerDao.UpdateAsync(customer);
        
        customer = await customerDao.FindByIdAsync(customerId);
        customer.Should().NotBeNull();
        customer!.Name.Should().Be("Alex Cossentine-Kofler");
    }
    
    [Fact]
    public async Task UpdateEntityWhenEntityDoesNotExist() {
        var customerId = Guid.Parse("c63b840a-a520-4a6a-a5d1-7328618c20c5");
        var customerDao = GetCustomerDao(ShopTenantId);
        var customer = await customerDao.FindByIdAsync(customerId);
        customer.Should().NotBeNull();
        customer!.Id.Should().Be("c63b840a-a520-4a6a-a5d1-7328618c20c5");
        customer = customer with {
                Id = Guid.Parse("f63b840a-a520-4a6a-a5d1-7328618c20c5"),
                Name = "Test Name"
        };
        
        Func<Task> act = async () => { await customerDao.UpdateAsync(customer); };
        await act.Should().ThrowAsync<CaasUpdateConcurrencyDbException>();
    }
    
    [Fact]
    public async Task DeleteEntityWhenEntityIsExisting() {
        var customerId = Guid.Parse("c63b840a-a520-4a6a-a5d1-7328618c20c5");
        var customerDao = GetCustomerDao(ShopTenantId);
        var customer = await customerDao.FindByIdAsync(customerId);
        customer.Should().NotBeNull();
        customer!.Name.Should().Be("Alex Cossentine");
        await customerDao.DeleteAsync(customer);
        
        customer = await customerDao.FindByIdAsync(customerId);
        customer.Should().BeNull();
    }
    
    private IDao<CustomerDataModel> GetCustomerDao(string tenantId) => GetDao(new CustomerDataRecordMapper(), tenantId);
}