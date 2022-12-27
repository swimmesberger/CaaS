using CaaS.Core.CustomerAggregate;
using CaaS.Infrastructure.Base.Tenant;
using CaaS.Infrastructure.CustomerData;
using CaaS.Infrastructure.ShopData;
using CaaS.Test.Common;

namespace CaaS.Test.ServiceTests; 

public class CustomerServiceTest {
    private static readonly Guid TestShopId = new Guid("1AF5037B-16A0-430A-8035-6BCD785CBFB6");
    private static readonly string TestShopName = "TestShop";
    private static readonly Guid TestShopAdminId = new Guid("B0077779-E33B-4DFF-938B-47BA4A10854B");
    private static readonly Guid CustomerIdA = new Guid("99C91EA1-4CA5-9097-DFDB-CF688F0DA31F");
    private static readonly Guid CustomerIdB = new Guid("9C0E8AA6-94C6-43F2-8446-D496DED2D7FE");

    [Fact]
    public async Task AddOptimistic() {
        var customerService = CreateCustomerService();
        var customer = new Customer {
            Id = CustomerIdB,
            Name = "Roman",
            ShopId = TestShopId
        };

        var result = await customerService.AddAsync(customer);
        result.Id.Should().Be(CustomerIdB);
        result.Name.Should().Be("Roman");
    }

    [Fact]
    public async Task FindByEmailOptimistic() {
        var customerService = CreateCustomerService();
        var customer = await customerService.GetByEmailAsync("simon@test.com");
        customer!.Id.Should().Be(CustomerIdB);

        customer = await customerService.GetByEmailAsync("lalala@test.at");
        customer.Should().BeNull();
    }

    private ICustomerService CreateCustomerService() {
        var customerDao = new MemoryDao<CustomerDataModel>(new List<CustomerDataModel>() {
            new CustomerDataModel { Id = CustomerIdA, ShopId = TestShopId, Name = "Roman Koho", EMail = "test@test.com", CreditCardNumber = "1111222233334444" },
            new CustomerDataModel { Id = CustomerIdB, ShopId = TestShopId, Name = "Simon Wimmesb", EMail = "simon@test.com", CreditCardNumber = "9999948945454" }
        });
        
        var customerRepository = new CustomerRepository(customerDao);
        
        var shopDao = new MemoryDao<ShopDataModel>(new List<ShopDataModel>() {
            new ShopDataModel() { Id = TestShopId, Name = TestShopName, AdminId = TestShopAdminId }
        });
        var shopAdminDao = new MemoryDao<ShopAdminDataModel>(new List<ShopAdminDataModel>() {
            new ShopAdminDataModel() { Id = TestShopAdminId, ShopId = TestShopId }
        });
        
        var shopRepository = new ShopRepository(shopDao, shopAdminDao);

        return new CustomerService(new StaticTenantIdAccessor(TestShopId.ToString()), customerRepository, shopRepository, new MockUnitOfWorkManager());
    }
}