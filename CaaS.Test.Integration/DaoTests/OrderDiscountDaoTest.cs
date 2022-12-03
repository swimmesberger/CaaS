using CaaS.Core.Base.Exceptions;
using CaaS.Infrastructure.Base.Ado;
using CaaS.Infrastructure.Base.Ado.Model;
using CaaS.Infrastructure.Base.Ado.Query.Parameters;
using CaaS.Infrastructure.Gen;
using CaaS.Infrastructure.OrderData;
using Xunit.Abstractions;

namespace CaaS.Test.Integration.DaoTests; 

public class OrderDiscountDaoTest : BaseDaoTest {
    private const string ShopTenantId = "a468d796-db09-496d-9794-f6b42f8c7c0b";

    public OrderDiscountDaoTest(ITestOutputHelper output) : base(output) { }
    
    [Fact]
    public async Task FindAllWhenDbHasEntries() {
        var orderDiscountDao = GetOrderDiscountDao(ShopTenantId);
        var orderDiscounts = await orderDiscountDao.FindAllAsync().ToListAsync();
        orderDiscounts[0].Id.Should().Be("ad6f89c5-68dc-499d-ae9e-0976cad053fd");
        orderDiscounts[1].Id.Should().Be("94c8b5ab-df5d-4aee-9391-d0189ef03fe4");
    }
    
    [Fact]
    public async Task FindByIdsWhenIdsAreValidReturnsEntities() {
        var orderDiscountDao = GetOrderDiscountDao(ShopTenantId);
        var idList = new List<Guid> {
                Guid.Parse("ad6f89c5-68dc-499d-ae9e-0976cad053fd"),
                Guid.Parse("94c8b5ab-df5d-4aee-9391-d0189ef03fe4")
        };
        
        var orderDiscounts = await orderDiscountDao.FindByIdsAsync(idList).ToListAsync();
        
        orderDiscounts[0].Id.Should().Be("ad6f89c5-68dc-499d-ae9e-0976cad053fd");
        orderDiscounts[1].Id.Should().Be("94c8b5ab-df5d-4aee-9391-d0189ef03fe4");
    }
    
    [Fact]
    public async Task FindByOrderIdAndNameReturnsEntities() {
        var orderDiscountDao = GetOrderDiscountDao(ShopTenantId);
        
        var parameters = new List<QueryParameter> {
            new(nameof(OrderDiscountDataModel.Id), Guid.Parse("94c8b5ab-df5d-4aee-9391-d0189ef03fe4")),
            new(nameof(OrderDiscountDataModel.DiscountName), "Black Friday"),
        };

        var orderDiscounts = await orderDiscountDao.FindBy(new StatementParameters { Where = parameters }).ToListAsync();
        
        orderDiscounts.Count.Should().NotBe(0);
        orderDiscounts[0].Id.Should().Be("94c8b5ab-df5d-4aee-9391-d0189ef03fe4");
    }
    
    [Fact]
    public async Task CountReturnsNumberOfEntitiesInDb() {
        var orderDiscountDao = GetOrderDiscountDao(ShopTenantId);
        (await orderDiscountDao.CountAsync()).Should().Be(2);
    }
    
    [Fact]
    public async Task AddAddsNewEntityToDb() {
        var orderDiscountDao = GetOrderDiscountDao(ShopTenantId);
        var orderDiscount = new OrderDiscountDataModel() {
                Id = Guid.Parse("7A819343-23A1-4AD9-8798-64D1047CF01F"),
                ShopId = Guid.Parse(ShopTenantId),
                DiscountName = "Special action",
                Discount = 10,
                OrderId = Guid.Parse("aca58853-bd61-4517-9907-ca51a50b7225")
        };
        await orderDiscountDao.AddAsync(orderDiscount);
        
        orderDiscount = await orderDiscountDao.FindByIdAsync(orderDiscount.Id);
        orderDiscount.Should().NotBeNull();
        orderDiscount!.Id.Should().Be("7A819343-23A1-4AD9-8798-64D1047CF01F");
    }

    [Fact]
    public async Task UpdateEntityWhenEntityIsExisting() {
        var orderDiscountId = Guid.Parse("94c8b5ab-df5d-4aee-9391-d0189ef03fe4");
        var orderDiscountDao = GetOrderDiscountDao(ShopTenantId);
        var orderDiscount = await orderDiscountDao.FindByIdAsync(orderDiscountId);
        orderDiscount.Should().NotBeNull();
        orderDiscount!.DiscountName.Should().Be("Black Friday");
        orderDiscount = orderDiscount with {
                DiscountName = "Test"
        };
        await orderDiscountDao.UpdateAsync(orderDiscount);
        
        orderDiscount = await orderDiscountDao.FindByIdAsync(orderDiscountId);
        orderDiscount.Should().NotBeNull();
        orderDiscount!.DiscountName.Should().Be("Test");
    }
    
    [Fact]
    public async Task UpdateEntityWhenEntityDoesNotExist() {
        var orderDiscountId = Guid.Parse("94c8b5ab-df5d-4aee-9391-d0189ef03fe4");
        var orderDiscountDao = GetOrderDiscountDao(ShopTenantId);
        var orderDiscount = await orderDiscountDao.FindByIdAsync(orderDiscountId);
        orderDiscount.Should().NotBeNull();
        orderDiscount!.Id.Should().Be("94c8b5ab-df5d-4aee-9391-d0189ef03fe4");
        orderDiscount = orderDiscount with {
                Id = Guid.Parse("993bffa6-e73d-4aa1-be30-aa636fa823c0"),
                Discount = 99
        };
        
        Func<Task> act = async () => { await orderDiscountDao.UpdateAsync(orderDiscount); };
        await act.Should().ThrowAsync<CaasUpdateConcurrencyDbException>();
    }
    
    [Fact]
    public async Task DeleteEntityWhenEntityIsExisting() {
        var orderDiscountId = Guid.Parse("94c8b5ab-df5d-4aee-9391-d0189ef03fe4");
        var orderDiscountDao = GetOrderDiscountDao(ShopTenantId);
        var orderDiscount = await orderDiscountDao.FindByIdAsync(orderDiscountId);
        orderDiscount.Should().NotBeNull();
        orderDiscount!.DiscountName.Should().Be("Black Friday");
        await orderDiscountDao.DeleteAsync(orderDiscount);
        
        orderDiscount = await orderDiscountDao.FindByIdAsync(orderDiscountId);
        orderDiscount.Should().BeNull();
    }
    
    private IDao<OrderDiscountDataModel> GetOrderDiscountDao(string tenantId) => GetDao(new OrderDiscountDataRecordMapper(), tenantId);
}