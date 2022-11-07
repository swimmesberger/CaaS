using CaaS.Core.Exceptions;
using CaaS.Infrastructure.Ado.Base;
using CaaS.Infrastructure.Ado.Model;
using CaaS.Infrastructure.DataModel;
using CaaS.Infrastructure.Gen;

namespace CaaS.Test.Integration.DaoTests; 

public class ProductOrderDaoTest : BaseDaoTest {
    private const string ShopTenantId = "a468d796-db09-496d-9794-f6b42f8c7c0b";
    
    [Fact]
    public async Task FindAllWhenDbHasEntries() {
        var productOrderDao = GetProductOrderDao(ShopTenantId);
        var orderItems = await productOrderDao.FindAllAsync().ToListAsync();
        orderItems[0].Id.Should().Be("363bffa6-e73d-4aa1-be30-aa636fa823c0");
        orderItems[1].Id.Should().Be("1f3c4771-7ec1-4017-a531-10dea11c0745");
    }

    [Fact]
    public async Task FindByIdsWhenIdsAreValidReturnsEntities() {
        var productOrderDao = GetProductOrderDao(ShopTenantId);
        var idList = new List<Guid> {
                Guid.Parse("363bffa6-e73d-4aa1-be30-aa636fa823c0"),
                Guid.Parse("1f3c4771-7ec1-4017-a531-10dea11c0745")
        };
        
        var orderItems = await productOrderDao.FindByIdsAsync(idList).ToListAsync();
        
        orderItems[0].Id.Should().Be("363bffa6-e73d-4aa1-be30-aa636fa823c0");
        orderItems[1].Id.Should().Be("1f3c4771-7ec1-4017-a531-10dea11c0745");
    }

    [Fact]
    public async Task FindByAmountReturnsEntities() {
        var productOrderDao = GetProductOrderDao(ShopTenantId);

        var parameters = new List<QueryParameter> {
            QueryParameter.From(nameof(ProductOrderDataModel.Amount), 4)
        };

        var orderItems = await productOrderDao.FindBy(StatementParameters
                .CreateWhere(parameters)).ToListAsync();

        orderItems.Count.Should().NotBe(0);
        orderItems.Count.Should().Be(2);
        orderItems[0].Amount.Should().Be(4);
        orderItems[1].Amount.Should().Be(4);
    }
    
    [Fact]
    public async Task CountReturnsNumberOfEntitiesInDb() {
        var productOrderDao = GetProductOrderDao(ShopTenantId);
        (await productOrderDao.CountAsync()).Should().Be(2);
    }

    [Fact]
    public async Task AddAddsNewEntityToDb() {
        var productOrderDao = GetProductOrderDao(ShopTenantId);
        var orderItem = new ProductOrderDataModel() {
                Id = Guid.Parse("7A819343-23A1-4AD9-8798-64D1047CF01F"),
                ProductId = Guid.Parse("587c3437-b430-405a-99dd-a0ce9ebde0a4"),
                OrderId = Guid.Parse("41b1fa55-fd34-4611-bda0-f3821f6db52b"),
                ShopId = Guid.Parse("a468d796-db09-496d-9794-f6b42f8c7c0b"),
                Amount = 10,
                PricePerPiece = (decimal)3.49
        };
        
        await productOrderDao.AddAsync(orderItem);
        
        orderItem = await productOrderDao.FindByIdAsync(orderItem.Id);
        orderItem.Should().NotBeNull();
        orderItem!.Id.Should().Be("7A819343-23A1-4AD9-8798-64D1047CF01F");
    }
    
    [Fact]
    public async Task UpdateEntityWhenEntityIsExisting() {
        var productOrderId = Guid.Parse("363bffa6-e73d-4aa1-be30-aa636fa823c0");
        var productOrderDao = GetProductOrderDao(ShopTenantId);
        var orderItem = await productOrderDao.FindByIdAsync(productOrderId);
        orderItem.Should().NotBeNull();
        orderItem!.Id.Should().Be("363bffa6-e73d-4aa1-be30-aa636fa823c0");
        orderItem = orderItem with {
            Amount = 99
        };
        await productOrderDao.UpdateAsync(orderItem);
        
        orderItem = await productOrderDao.FindByIdAsync(productOrderId);
        orderItem.Should().NotBeNull();
        orderItem!.Amount.Should().Be(99);
    }
    
    [Fact]
    public async Task UpdateEntityWhenEntityDoesNotExist() {
        var productOrderId = Guid.Parse("363bffa6-e73d-4aa1-be30-aa636fa823c0");
        var productOrderDao = GetProductOrderDao(ShopTenantId);
        var orderItem = await productOrderDao.FindByIdAsync(productOrderId);
        orderItem.Should().NotBeNull();
        orderItem!.Id.Should().Be("363bffa6-e73d-4aa1-be30-aa636fa823c0");
        orderItem = orderItem with {
                Id = Guid.Parse("993bffa6-e73d-4aa1-be30-aa636fa823c0"),
                Amount = 99
        };
        
        Func<Task> act = async () => { await productOrderDao.UpdateAsync(orderItem); };
        await act.Should().ThrowAsync<CaasUpdateConcurrencyDbException>();
    }
    
    [Fact]
    public async Task DeleteEntityWhenEntityIsExisting() {
        var productOrderId = Guid.Parse("363bffa6-e73d-4aa1-be30-aa636fa823c0");
        var productOrderDao = GetProductOrderDao(ShopTenantId);
        var orderItem = await productOrderDao.FindByIdAsync(productOrderId);
        orderItem.Should().NotBeNull();
        orderItem!.Id.Should().Be("363bffa6-e73d-4aa1-be30-aa636fa823c0");
        await productOrderDao.DeleteAsync(orderItem);
        
        orderItem = await productOrderDao.FindByIdAsync(productOrderId);
        orderItem.Should().BeNull();
    }
    
    private IDao<ProductOrderDataModel> GetProductOrderDao(string tenantId) => GetDao(new ProductOrderDataRecordMapper(), tenantId);
}