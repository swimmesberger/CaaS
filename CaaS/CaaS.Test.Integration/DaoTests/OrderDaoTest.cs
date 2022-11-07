using CaaS.Core.Entities;
using CaaS.Core.Exceptions;
using CaaS.Infrastructure.Ado.Base;
using CaaS.Infrastructure.Ado.Model;
using CaaS.Infrastructure.DataModel;
using CaaS.Infrastructure.Gen;

namespace CaaS.Test.Integration; 

public class OrderDaoTest : BaseDaoTest {
    private const string ShopTenantId = "a468d796-db09-496d-9794-f6b42f8c7c0b";
    
    [Fact]
    public async Task FindAllWhenDbHasEntries() {
        var orderDao = GetOrderDao(ShopTenantId);
        var orders = await orderDao.FindAllAsync().ToListAsync();
        orders[0].Id.Should().Be("41b1fa55-fd34-4611-bda0-f3821f6db52b");
        orders[1].Id.Should().Be("aca58853-bd61-4517-9907-ca51a50b7225");
    }
    
    [Fact]
    public async Task FindByIdsWhenIdsAreValidReturnsEntities() {
        var orderDao = GetOrderDao(ShopTenantId);
        var idList = new List<Guid> {
                Guid.Parse("41b1fa55-fd34-4611-bda0-f3821f6db52b"),
                Guid.Parse("aca58853-bd61-4517-9907-ca51a50b7225")
        };
        
        var orders = await orderDao.FindByIdsAsync(idList).ToListAsync();
        
        orders[0].Id.Should().Be("41b1fa55-fd34-4611-bda0-f3821f6db52b");
        orders[1].Id.Should().Be("aca58853-bd61-4517-9907-ca51a50b7225");
    }
    
    [Fact]
    public async Task FindByOrderIdAndNameReturnsEntities() {
        var orderDao = GetOrderDao(ShopTenantId);
        
        var parameters = new List<QueryParameter> {
                new(nameof(Order.Id), Guid.Parse("aca58853-bd61-4517-9907-ca51a50b7225")),
                new(nameof(Order.OrderNumber), 7785),
        };

        var products = await orderDao.FindBy(StatementParameters
                .CreateWhere(parameters)).ToListAsync();
        
        products.Count.Should().NotBe(0);
        products[0].Id.Should().Be("aca58853-bd61-4517-9907-ca51a50b7225");
    }
    
    [Fact]
    public async Task CountReturnsNumberOfEntitiesInDb() {
        var orderDao = GetOrderDao(ShopTenantId);
        (await orderDao.CountAsync()).Should().Be(2);
    }
    
    [Fact]
    public async Task AddAddsNewEntityToDb() {
        var orderDao = GetOrderDao(ShopTenantId);
        var order = new OrderDataModel() {
                Id = Guid.Parse("7A819343-23A1-4AD9-8798-64D1047CF01F"),
                ShopId = Guid.Parse(ShopTenantId),
                OrderNumber = 49538,
                CustomerId = Guid.Parse("9234a988-0abd-4b44-808a-9e7a8852e19c"),
                OrderDate = DateTimeOffset.Now
        };
        await orderDao.AddAsync(order);
        
        order = await orderDao.FindByIdAsync(order.Id);
        order.Should().NotBeNull();
        order!.Id.Should().Be("7A819343-23A1-4AD9-8798-64D1047CF01F");
    }

    [Fact]
    public async Task UpdateEntityWhenEntityIsExisting() {
        var orderId = Guid.Parse("41b1fa55-fd34-4611-bda0-f3821f6db52b");
        var orderDao = GetOrderDao(ShopTenantId);
        var order = await orderDao.FindByIdAsync(orderId);
        order.Should().NotBeNull();
        order!.OrderNumber.Should().Be(2628);
        order = order with {
                OrderNumber = 20000
        };
        await orderDao.UpdateAsync(order);
        
        order = await orderDao.FindByIdAsync(orderId);
        order.Should().NotBeNull();
        order!.OrderNumber.Should().Be(20000);
    }
    
    [Fact]
    public async Task UpdateEntityWhenEntityDoesNotExist() {
        var orderId = Guid.Parse("41b1fa55-fd34-4611-bda0-f3821f6db52b");
        var orderDao = GetOrderDao(ShopTenantId);
        var order = await orderDao.FindByIdAsync(orderId);
        order.Should().NotBeNull();
        order!.Id.Should().Be("41b1fa55-fd34-4611-bda0-f3821f6db52b");
        order = order with {
                Id = Guid.Parse("993bffa6-e73d-4aa1-be30-aa636fa823c0"),
                OrderNumber = 99
        };
        
        Func<Task> act = async () => { await orderDao.UpdateAsync(order); };
        await act.Should().ThrowAsync<CaasUpdateConcurrencyDbException>();
    }
    
    [Fact]
    public async Task DeleteEntityWhenEntityIsExisting() {
        var orderId = Guid.Parse("aca58853-bd61-4517-9907-ca51a50b7225");
        var orderDao = GetOrderDao(ShopTenantId);
        var order = await orderDao.FindByIdAsync(orderId);
        order.Should().NotBeNull();
        order!.OrderNumber.Should().Be(7785);
        await orderDao.DeleteAsync(order);
        
        order = await orderDao.FindByIdAsync(orderId);
        order.Should().BeNull();
    }
    
    private IDao<OrderDataModel> GetOrderDao(string tenantId) => GetDao(new OrderDataRecordMapper(), tenantId);
}