using CaaS.Core.Base;
using CaaS.Core.Base.Exceptions;
using CaaS.Core.OrderAggregate;
using CaaS.Infrastructure.Base.Ado;
using CaaS.Infrastructure.Base.Ado.Model;
using CaaS.Infrastructure.Base.Ado.Model.Where;
using CaaS.Infrastructure.Gen;
using CaaS.Infrastructure.OrderData;
using Xunit.Abstractions;

namespace CaaS.Test.Integration.DaoTests; 

public class OrderDaoTest : BaseDaoTest {
    private const string ShopTenantId = "a468d796-db09-496d-9794-f6b42f8c7c0b";

    public OrderDaoTest(ITestOutputHelper output) : base(output) { }
    
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
    public async Task FindByOrderIdAndNumberReturnsEntities() {
        var orderDao = GetOrderDao(ShopTenantId);
        
        var parameters = new List<QueryParameter> {
            QueryParameter.From(nameof(Order.Id), Guid.Parse("aca58853-bd61-4517-9907-ca51a50b7225")),
            QueryParameter.From(nameof(Order.OrderNumber), 7785),
        };

        var products = await orderDao.FindBy(StatementParameters
                .CreateWhere(parameters)).ToListAsync();
        
        products.Count.Should().NotBe(0);
        products[0].Id.Should().Be("aca58853-bd61-4517-9907-ca51a50b7225");
    }
    
    [Fact]
    public async Task FindByOrderNumberLessAndGreater() {
        var orderDao = GetOrderDao(ShopTenantId);
        
        var parameters = new List<QueryParameter> {
            QueryParameter.From(nameof(Order.OrderNumber), 2628 , comparator: WhereComparator.GreaterOrEqual),
            QueryParameter.From(nameof(Order.OrderNumber), 3000 , comparator: WhereComparator.LessOrEqual),
        };

        var products = await orderDao.FindBy(StatementParameters
            .CreateWhere(parameters)).ToListAsync();
        
        products.Count.Should().NotBe(0);
        products.Count.Should().Be(1);
        products[0].Id.Should().Be("41b1fa55-fd34-4611-bda0-f3821f6db52b");
    }
    
    [Fact]
    public async Task FindByOrderDate() {
        var orderDao = GetOrderDao(ShopTenantId);
        DateTimeOffset date = DateTimeOffset.Parse("2022-07-18 09:38:13+00");

        var parameters = new List<QueryParameter> {
            QueryParameter.From(nameof(Order.OrderDate), date),
        };

        var products = await orderDao.FindBy(StatementParameters
            .CreateWhere(parameters)).ToListAsync();
        
        products.Count.Should().NotBe(0);
        products.Count.Should().Be(1);
        products[0].Id.Should().Be("41b1fa55-fd34-4611-bda0-f3821f6db52b");
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
                OrderDate = DateTimeOffsetProvider.GetNow()
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

    [Fact]
    public async Task DeleteExistingEntityCheckOrderItemFk() {
        var orderDao = GetOrderDao(ShopTenantId);
        var orderId = Guid.Parse("41b1fa55-fd34-4611-bda0-f3821f6db52b");
        var order = await orderDao.FindByIdAsync(orderId);
        order.Should().NotBeNull();
        order!.OrderNumber.Should().Be(2628);

        var orderItemDao = GetOrderItemDao(ShopTenantId);
        var orderItem1Id = Guid.Parse("363bffa6-e73d-4aa1-be30-aa636fa823c0");
        var orderItem2Id = Guid.Parse("1f3c4771-7ec1-4017-a531-10dea11c0745");
        var orderItemIds = new List<Guid> { orderItem1Id, orderItem2Id };
        var orderItems = await orderItemDao.FindByIdsAsync(orderItemIds).ToListAsync();
        orderItems[0].Id.Should().Be(orderItem1Id);
        orderItems[1].Id.Should().Be(orderItem2Id);

        await orderDao.DeleteAsync(order);
        orderItems = await orderItemDao.FindByIdsAsync(orderItemIds).ToListAsync();
        orderItems.Should().BeEmpty();
    }
    
    [Fact]
    public async Task AddBulkAddsNewEntitiesToDb() {
        var id1 = Guid.Parse("7A819343-23A1-4AD9-8798-64D1047CF01F");
        var id2 = Guid.Parse("9234a988-0abd-4b44-808a-9e7a8852e19c");
        
        var orderDao = GetOrderDao(ShopTenantId);
        var order1 = new OrderDataModel() {
            Id = id1,
            ShopId = Guid.Parse(ShopTenantId),
            OrderNumber = 49538,
            CustomerId = id2,
            OrderDate = DateTimeOffsetProvider.GetNow()
        };
        var order2 = new OrderDataModel() {
            Id = id2,
            ShopId = Guid.Parse(ShopTenantId),
            OrderNumber = 102,
            CustomerId = id2,
            OrderDate = DateTimeOffsetProvider.GetNow()
        };
        var entities = new List<OrderDataModel> { order1, order2 };
        await orderDao.AddAsync(entities);

        var orders =
            await orderDao.FindByIdsAsync(new List<Guid> { id1, id2,}).ToListAsync();

        orders.Should().NotBeNull();
        orders[0].Id.Should().Be(id1);
        orders[1].Id.Should().Be(id2);
    }

    [Fact]
    public async Task BulkUpdateUpdatesExistingEntities() {
        var id1 = Guid.Parse("41b1fa55-fd34-4611-bda0-f3821f6db52b");
        var id2 = Guid.Parse("aca58853-bd61-4517-9907-ca51a50b7225");
        var orderDao = GetOrderDao(ShopTenantId);

        var order1 = await orderDao.FindByIdAsync(id1);
        var order2 = await orderDao.FindByIdAsync(id2);

        order1.Should().NotBeNull();
        order1!.OrderNumber.Should().Be(2628);
        order1 = order1 with {
            OrderNumber = 99
        };

        order2.Should().NotBeNull();
        order2!.OrderNumber.Should().Be(7785);
        order2 = order2 with {
            OrderNumber = 100
        };
        
        await orderDao.UpdateAsync(new List<OrderDataModel> {order1, order2});
        
        order1 = await orderDao.FindByIdAsync(id1);
        order2 = await orderDao.FindByIdAsync(id2);
        
        order1.Should().NotBeNull();
        order1!.OrderNumber.Should().Be(99);
        
        order2.Should().NotBeNull();
        order2!.OrderNumber.Should().Be(100);
    }

    [Fact]
    public async Task BulkDeleteDeletesExistingEntities() {
        var orderDao = GetOrderDao(ShopTenantId);
        var id1 = Guid.Parse("41b1fa55-fd34-4611-bda0-f3821f6db52b");
        var id2 = Guid.Parse("aca58853-bd61-4517-9907-ca51a50b7225");
        var nonExistingId = Guid.Parse("aef11853-bd61-4517-9907-ca51a50b7225");
        
        var ids = new List<Guid> { id1, id2, nonExistingId };
        var orders = await orderDao.FindByIdsAsync(ids).ToListAsync();
        orders.Count.Should().Be(2);
        orders[0].Id.Should().Be(id1);
        orders[1].Id.Should().Be(id2);
        
        await orderDao.DeleteAsync(orders);
        orders = await orderDao.FindByIdsAsync(ids).ToListAsync();
        orders.Should().BeEmpty();
    }


    private IDao<OrderDataModel> GetOrderDao(string tenantId) => GetDao(new OrderDataRecordMapper(), tenantId);
    private IDao<ProductOrderDataModel> GetOrderItemDao(string tenantId) => GetDao(new ProductOrderDataRecordMapper(), tenantId);
}