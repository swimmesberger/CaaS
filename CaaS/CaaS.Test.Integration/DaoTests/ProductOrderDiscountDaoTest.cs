using CaaS.Core.Exceptions;
using CaaS.Infrastructure.Ado.Base;
using CaaS.Infrastructure.Ado.Model;
using CaaS.Infrastructure.DataModel;
using CaaS.Infrastructure.Gen;

namespace CaaS.Test.Integration.DaoTests; 

public class ProductOrderDiscountDaoTest : BaseDaoTest {
    private const string ShopTenantId = "a468d796-db09-496d-9794-f6b42f8c7c0b";
    
    [Fact]
    public async Task FindAllWhenDbHasEntries() {
        var productOrderDiscountDao = GetProductOrderDiscountDao(ShopTenantId);
        var orderItemDiscounts = await productOrderDiscountDao.FindAllAsync().ToListAsync();
        orderItemDiscounts[0].Id.Should().Be("b7ab7819-6f94-467c-ac2e-79d857664ee7");
        orderItemDiscounts[1].Id.Should().Be("8ac6ba97-921f-45f6-a8dd-a89d688cce83");
    }
    
    [Fact]
    public async Task FindByIdsWhenIdsAreValidReturnsEntities() {
        var productOrderDiscountDao = GetProductOrderDiscountDao(ShopTenantId);
        var idList = new List<Guid> {
                Guid.Parse("b7ab7819-6f94-467c-ac2e-79d857664ee7"),
                Guid.Parse("8ac6ba97-921f-45f6-a8dd-a89d688cce83")
        };
        
        var orderItemDiscounts = await productOrderDiscountDao.FindByIdsAsync(idList).ToListAsync();
        
        orderItemDiscounts[0].Id.Should().Be("b7ab7819-6f94-467c-ac2e-79d857664ee7");
        orderItemDiscounts[1].Id.Should().Be("8ac6ba97-921f-45f6-a8dd-a89d688cce83");
    }
    
    [Fact]
    public async Task FindByDiscountNameReturnsEntities() {
        var productOrderDiscountDao = GetProductOrderDiscountDao(ShopTenantId);
        
        var parameters = new List<QueryParameter> {
            QueryParameter.From(nameof(ProductOrderDiscountDataModel.DiscountName), "-10% on everything"),
        };

        var orderItemDiscounts = await productOrderDiscountDao.FindBy(StatementParameters
                .CreateWhere(parameters)).ToListAsync();
        
        orderItemDiscounts.Count.Should().NotBe(0);
        orderItemDiscounts[0].Id.Should().Be("b7ab7819-6f94-467c-ac2e-79d857664ee7");
    }
    
    [Fact]
    public async Task CountReturnsNumberOfEntitiesInDb() {
        var productOrderDiscountDao = GetProductOrderDiscountDao(ShopTenantId);
        (await productOrderDiscountDao.CountAsync()).Should().Be(2);
    }
    
    [Fact]
    public async Task AddAddsNewEntityToDb() {
        var productOrderDiscountDao = GetProductOrderDiscountDao(ShopTenantId);
        var orderItemDiscount = new ProductOrderDiscountDataModel() {
                Id = Guid.Parse("7A819343-23A1-4AD9-8798-64D1047CF01F"),
                ShopId = Guid.Parse(ShopTenantId),
                DiscountName = "Special action",
                Discount = 10,
                ProductOrderId = Guid.Parse("363bffa6-e73d-4aa1-be30-aa636fa823c0")
        };
        await productOrderDiscountDao.AddAsync(orderItemDiscount);
        
        orderItemDiscount = await productOrderDiscountDao.FindByIdAsync(orderItemDiscount.Id);
        orderItemDiscount.Should().NotBeNull();
        orderItemDiscount!.Id.Should().Be("7A819343-23A1-4AD9-8798-64D1047CF01F");
    }

    [Fact]
    public async Task UpdateEntityWhenEntityIsExisting() {
        var productOrderDiscountId = Guid.Parse("b7ab7819-6f94-467c-ac2e-79d857664ee7");
        var productOrderDiscountDao = GetProductOrderDiscountDao(ShopTenantId);
        var orderItemDiscount = await productOrderDiscountDao.FindByIdAsync(productOrderDiscountId);
        orderItemDiscount.Should().NotBeNull();
        orderItemDiscount!.DiscountName.Should().Be("-10% on everything");
        orderItemDiscount = orderItemDiscount with {
                DiscountName = "Test"
        };
        await productOrderDiscountDao.UpdateAsync(orderItemDiscount);
        
        orderItemDiscount = await productOrderDiscountDao.FindByIdAsync(productOrderDiscountId);
        orderItemDiscount.Should().NotBeNull();
        orderItemDiscount!.DiscountName.Should().Be("Test");
    }
    
    [Fact]
    public async Task UpdateEntityWhenEntityDoesNotExist() {
        var productOrderDiscountId = Guid.Parse("b7ab7819-6f94-467c-ac2e-79d857664ee7");
        var productOrderDiscountDao = GetProductOrderDiscountDao(ShopTenantId);
        var productOrderDiscount = await productOrderDiscountDao.FindByIdAsync(productOrderDiscountId);
        productOrderDiscount.Should().NotBeNull();
        productOrderDiscount!.Id.Should().Be("b7ab7819-6f94-467c-ac2e-79d857664ee7");
        productOrderDiscount = productOrderDiscount with {
                Id = Guid.Parse("993bffa6-e73d-4aa1-be30-aa636fa823c0"),
                Discount = 99
        };
        
        Func<Task> act = async () => { await productOrderDiscountDao.UpdateAsync(productOrderDiscount); };
        await act.Should().ThrowAsync<CaasUpdateConcurrencyDbException>();
    }
    
    [Fact]
    public async Task DeleteEntityWhenEntityIsExisting() {
        var productOrderDiscountId = Guid.Parse("b7ab7819-6f94-467c-ac2e-79d857664ee7");
        var productOrderDiscountDao = GetProductOrderDiscountDao(ShopTenantId);
        var productOrderDiscount = await productOrderDiscountDao.FindByIdAsync(productOrderDiscountId);
        productOrderDiscount.Should().NotBeNull();
        productOrderDiscount!.DiscountName.Should().Be("-10% on everything");
        await productOrderDiscountDao.DeleteAsync(productOrderDiscount);
        
        productOrderDiscount = await productOrderDiscountDao.FindByIdAsync(productOrderDiscountId);
        productOrderDiscount.Should().BeNull();
    }
    
    private IDao<ProductOrderDiscountDataModel> GetProductOrderDiscountDao(string tenantId) => GetDao(new ProductOrderDiscountDataRecordMapper(), tenantId);
}