using CaaS.Core.Exceptions;
using CaaS.Infrastructure.Ado.Base;
using CaaS.Infrastructure.Ado.Model;
using CaaS.Infrastructure.DataModel;
using CaaS.Infrastructure.Gen;

namespace CaaS.Test.Integration.DaoTests; 

public class ProductCartDaoTest : BaseDaoTest {
    private const string ShopTenantId = "a468d796-db09-496d-9794-f6b42f8c7c0b";
    
    [Fact]
    public async Task FindAllWhenDbHasEntries() {
        var productCartDao = GetProductCartDao(ShopTenantId);
        var cartItems = await productCartDao.FindAllAsync().ToListAsync();
        cartItems[0].Id.Should().Be("f4ef7180-016b-400a-a24e-797b7feb7780");
        cartItems[1].Id.Should().Be("77894ac7-e360-44b2-a873-2710b3114fdf");
    }

    [Fact]
    public async Task FindByIdsWhenIdsAreValidReturnsEntities() {
        var productCartDao = GetProductCartDao(ShopTenantId);
        var idList = new List<Guid> {
                Guid.Parse("f4ef7180-016b-400a-a24e-797b7feb7780"),
                Guid.Parse("77894ac7-e360-44b2-a873-2710b3114fdf")
        };
        
        var cartItems = await productCartDao.FindByIdsAsync(idList).ToListAsync();
        
        cartItems[0].Id.Should().Be("f4ef7180-016b-400a-a24e-797b7feb7780");
        cartItems[1].Id.Should().Be("77894ac7-e360-44b2-a873-2710b3114fdf");
    }

    [Fact]
    public async Task FindByAmountReturnsEntities() {
        var productCartDao = GetProductCartDao(ShopTenantId);

        var parameters = new List<QueryParameter> {
            QueryParameter.From(nameof(ProductCartDataModel.Amount), 4)
        };

        var cartItems = await productCartDao.FindBy(StatementParameters
                .CreateWhere(parameters)).ToListAsync();

        cartItems.Count.Should().NotBe(0);
        cartItems.Count.Should().Be(2);
        cartItems[0].Amount.Should().Be(4);
        cartItems[1].Amount.Should().Be(4);
    }
    
    [Fact]
    public async Task CountReturnsNumberOfEntitiesInDb() {
        var productCartDao = GetProductCartDao(ShopTenantId);
        (await productCartDao.CountAsync()).Should().Be(2);
    }

    [Fact]
    public async Task AddAddsNewEntityToDb() {
        var productCartDao = GetProductCartDao(ShopTenantId);
        var cartItem = new ProductCartDataModel() {
                Id = Guid.Parse("7A819343-23A1-4AD9-8798-64D1047CF01F"),
                ShopId = Guid.Parse("a468d796-db09-496d-9794-f6b42f8c7c0b"),
                ProductId = Guid.Parse("587c3437-b430-405a-99dd-a0ce9ebde0a4"),
                CartId = Guid.Parse("b4cf2977-13fc-44dd-89cf-0dbfdae04fce"),
                Amount = 10,
        };
        
        await productCartDao.AddAsync(cartItem);
        
        cartItem = await productCartDao.FindByIdAsync(cartItem.Id);
        cartItem.Should().NotBeNull();
        cartItem!.Id.Should().Be("7A819343-23A1-4AD9-8798-64D1047CF01F");
    }
    
    [Fact]
    public async Task UpdateEntityWhenEntityIsExisting() {
        var cartItemId = Guid.Parse("77894ac7-e360-44b2-a873-2710b3114fdf");
        var productCartDao = GetProductCartDao(ShopTenantId);
        var cartItem = await productCartDao.FindByIdAsync(cartItemId);
        cartItem.Should().NotBeNull();
        cartItem!.Id.Should().Be("77894ac7-e360-44b2-a873-2710b3114fdf");
        cartItem = cartItem with {
            Amount = 99
        };
        await productCartDao.UpdateAsync(cartItem);
        
        cartItem = await productCartDao.FindByIdAsync(cartItemId);
        cartItem.Should().NotBeNull();
        cartItem!.Amount.Should().Be(99);
    }
    
    [Fact]
    public async Task UpdateEntityWhenEntityDoesNotExist() {
        var cartItemId = Guid.Parse("77894ac7-e360-44b2-a873-2710b3114fdf");
        var productCartDao = GetProductCartDao(ShopTenantId);
        var cartItem = await productCartDao.FindByIdAsync(cartItemId);
        cartItem.Should().NotBeNull();
        cartItem!.Id.Should().Be("77894ac7-e360-44b2-a873-2710b3114fdf");
        cartItem = cartItem with {
                Id = Guid.Parse("993bffa6-e73d-4aa1-be30-aa636fa823c0"),
                Amount = 99
        };
        
        Func<Task> act = async () => { await productCartDao.UpdateAsync(cartItem); };
        await act.Should().ThrowAsync<CaasUpdateConcurrencyDbException>();
    }
    
    [Fact]
    public async Task DeleteEntityWhenEntityIsExisting() {
        var productCartId = Guid.Parse("77894ac7-e360-44b2-a873-2710b3114fdf");
        var productCartDao = GetProductCartDao(ShopTenantId);
        var cartItem = await productCartDao.FindByIdAsync(productCartId);
        cartItem.Should().NotBeNull();
        cartItem!.Id.Should().Be("77894ac7-e360-44b2-a873-2710b3114fdf");
        await productCartDao.DeleteAsync(cartItem);
        
        cartItem = await productCartDao.FindByIdAsync(productCartId);
        cartItem.Should().BeNull();
    }
    
        
    [Fact]
    public async Task DeleteMultipleEntitiesBatch() {
        var productDao = GetProductCartDao(ShopTenantId);
        var products = await productDao.FindAllAsync().ToListAsync();
        await productDao.DeleteAsync(products);
        products = await productDao.FindAllAsync().ToListAsync();
        products.Should().BeEmpty();
    }
    
    private IDao<ProductCartDataModel> GetProductCartDao(string tenantId) => GetDao(new ProductCartDataRecordMapper(), tenantId);
}