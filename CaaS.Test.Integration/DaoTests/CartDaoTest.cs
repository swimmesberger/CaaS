using CaaS.Core.Exceptions;
using CaaS.Infrastructure.Base.Ado;
using CaaS.Infrastructure.Base.Ado.Model;
using CaaS.Infrastructure.Cart.DataModel;
using CaaS.Infrastructure.Gen;
using Xunit.Abstractions;

namespace CaaS.Test.Integration.DaoTests;

public class CartDaoTest : BaseDaoTest {
    private const string ShopTenantId = "a468d796-db09-496d-9794-f6b42f8c7c0b";

    public CartDaoTest(ITestOutputHelper output) : base(output) { }
    
    [Fact]
    public async Task FindAllWhenDbHasEntries() {
        var cartDao = GetCartDao(ShopTenantId);
        var carts = await cartDao.FindAllAsync().ToListAsync();
        carts[0].Id.Should().Be("b4cf2977-13fc-44dd-89cf-0dbfdae04fce");
        carts[1].Id.Should().Be("4c99b9b8-9cb5-4a49-ada9-01bd95f398d4");
    }

    [Fact]
    public async Task FindByIdsWhenIdsAreValidReturnsEntities() {
        var cartDao = GetCartDao(ShopTenantId);
        var idList = new List<Guid> {
            Guid.Parse("b4cf2977-13fc-44dd-89cf-0dbfdae04fce"),
            Guid.Parse("4c99b9b8-9cb5-4a49-ada9-01bd95f398d4")
        };

        var carts = await cartDao.FindByIdsAsync(idList).ToListAsync();

        carts[0].Id.Should().Be("b4cf2977-13fc-44dd-89cf-0dbfdae04fce");
        carts[1].Id.Should().Be("4c99b9b8-9cb5-4a49-ada9-01bd95f398d4");
    }

    [Fact]
    public async Task FindByCustomerIdReturnsEntities() {
        var cartDao = GetCartDao(ShopTenantId);

        var parameters = new List<QueryParameter> {
            QueryParameter.From(nameof(CartDataModel.CustomerId), null)
        };

        var carts = await cartDao.FindBy(StatementParameters
            .CreateWhere(parameters)).ToListAsync();

        carts.Count.Should().NotBe(0);
        carts.Count.Should().Be(1);
        carts[0].Id.Should().Be("4c99b9b8-9cb5-4a49-ada9-01bd95f398d4");
    }

    [Fact]
    public async Task CountReturnsNumberOfEntitiesInDb() {
        var cartDao = GetCartDao(ShopTenantId);
        (await cartDao.CountAsync()).Should().Be(2);
    }

    [Fact]
    public async Task AddAddsNewEntityToDb() {
        var cartDao = GetCartDao(ShopTenantId);
        var cart = new CartDataModel() {
            Id = Guid.Parse("b52d92f6-f453-4e6b-be9d-a159a427108d"),
            ShopId = Guid.Parse(ShopTenantId),
            CustomerId = Guid.Parse("9234a988-0abd-4b44-808a-9e7a8852e19c"),
            LastAccess = DateTimeOffset.UtcNow
        };
        await cartDao.AddAsync(cart);

        cart = await cartDao.FindByIdAsync(cart.Id);
        cart.Should().NotBeNull();
        cart!.Id.Should().Be("b52d92f6-f453-4e6b-be9d-a159a427108d");
    }

    [Fact]
    public async Task UpdateEntityWhenEntityIsExisting() {
        var cartId = Guid.Parse("4c99b9b8-9cb5-4a49-ada9-01bd95f398d4");
        var cartDao = GetCartDao(ShopTenantId);
        var cart = await cartDao.FindByIdAsync(cartId);
        cart.Should().NotBeNull();
        cart!.CustomerId.Should().Be(null);
        cart = cart with {
            CustomerId = Guid.Parse("c63b840a-a520-4a6a-a5d1-7328618c20c5")
        };
        await cartDao.UpdateAsync(cart);

        cart = await cartDao.FindByIdAsync(cartId);
        cart.Should().NotBeNull();
        cart!.CustomerId.Should().Be("c63b840a-a520-4a6a-a5d1-7328618c20c5");
    }

    [Fact]
    public async Task UpdateEntityWhenEntityDoesNotExist() {
        var cartId = Guid.Parse("4c99b9b8-9cb5-4a49-ada9-01bd95f398d4");
        var cartDao = GetCartDao(ShopTenantId);
        var cart = await cartDao.FindByIdAsync(cartId);
        cart.Should().NotBeNull();
        cart!.Id.Should().Be("4c99b9b8-9cb5-4a49-ada9-01bd95f398d4");
        cart = cart with {
            Id = Guid.Parse("993bffa6-e73d-4aa1-be30-aa636fa823c0"),
            CustomerId = null
        };

        Func<Task> act = async () => { await cartDao.UpdateAsync(cart); };
        await act.Should().ThrowAsync<CaasUpdateConcurrencyDbException>();
    }

    [Fact]
    public async Task DeleteEntityWhenEntityIsExisting() {
        var orderId = Guid.Parse("4c99b9b8-9cb5-4a49-ada9-01bd95f398d4");
        var cartDao = GetCartDao(ShopTenantId);
        var cart = await cartDao.FindByIdAsync(orderId);
        cart.Should().NotBeNull();
        cart!.Id.Should().Be("4c99b9b8-9cb5-4a49-ada9-01bd95f398d4");
        await cartDao.DeleteAsync(cart);

        cart = await cartDao.FindByIdAsync(orderId);
        cart.Should().BeNull();
    }
    
        [Fact]
    public async Task DeleteExistingEntityCheckOrderItemFk() {
        var cartDao = GetCartDao(ShopTenantId);
        var cartId = Guid.Parse("b4cf2977-13fc-44dd-89cf-0dbfdae04fce");
        var cart = await cartDao.FindByIdAsync(cartId);
        cart.Should().NotBeNull();
        cart!.Id.Should().Be(cartId);

        var cartItemDao = GetCartItemDao(ShopTenantId);
        var cartItem1Id = Guid.Parse("f4ef7180-016b-400a-a24e-797b7feb7780");
        var cartItem2Id = Guid.Parse("77894ac7-e360-44b2-a873-2710b3114fdf");
        
        var cartItemIds = new List<Guid> { cartItem1Id, cartItem2Id };
        var cartItems = await cartItemDao.FindByIdsAsync(cartItemIds).ToListAsync();
        cartItems.Should().NotBeEmpty();
        cartItems[0].Id.Should().Be(cartItem1Id);
        cartItems[1].Id.Should().Be(cartItem2Id);

        await cartDao.DeleteAsync(cart);
        cartItems = await cartItemDao.FindByIdsAsync(cartItemIds).ToListAsync();
        cartItems.Should().BeEmpty();
    }
    
    [Fact]
    public async Task AddBulkAddsNewEntitiesToDb() {
        var id1 = Guid.Parse("7A819343-23A1-4AD9-8798-64D1047CF01F");
        var id2 = Guid.Parse("9234a988-0abd-4b44-808a-9e7a8852e19c");
        
        var cartDao = GetCartDao(ShopTenantId);
        var cart1 = new CartDataModel() {
            Id = id1,
            ShopId = Guid.Parse(ShopTenantId),
            CustomerId = Guid.Parse("9234a988-0abd-4b44-808a-9e7a8852e19c"),
            LastAccess = DateTimeOffset.UtcNow
        };
        var cart2 = new CartDataModel() {
            Id = id2,
            ShopId = Guid.Parse(ShopTenantId),
            CustomerId = Guid.Parse("9234a988-0abd-4b44-808a-9e7a8852e19c"),
            LastAccess = DateTimeOffset.UtcNow
        };
        var entities = new List<CartDataModel> { cart1, cart2 };
        await cartDao.AddAsync(entities);
    
        var carts =
            await cartDao.FindByIdsAsync(new List<Guid> { id1, id2,}).ToListAsync();
    
        carts.Should().NotBeNull();
        carts[0].Id.Should().Be(id1);
        carts[1].Id.Should().Be(id2);
    }
    
    [Fact]
    public async Task BulkUpdateUpdatesExistingEntities() {
        var customerId1 = Guid.Parse("9234a988-0abd-4b44-808a-9e7a8852e19c");
        var customerId2 = Guid.Parse("703111b0-c3fd-4bf1-9d1a-12cd3852c182");

        var id1 = Guid.Parse("b4cf2977-13fc-44dd-89cf-0dbfdae04fce");
        var id2 = Guid.Parse("4c99b9b8-9cb5-4a49-ada9-01bd95f398d4");
        var cartDao = GetCartDao(ShopTenantId);
    
        var cart1 = await cartDao.FindByIdAsync(id1);
        var cart2 = await cartDao.FindByIdAsync(id2);
    
        cart1.Should().NotBeNull();
        cart1!.CustomerId.Should().Be(customerId2);
        cart1 = cart1 with {
            CustomerId = null
        };
    
        cart2.Should().NotBeNull();
        cart2!.CustomerId.Should().Be(null);
        cart2 = cart2 with {
            CustomerId = customerId1
        };
        
        await cartDao.UpdateAsync(new List<CartDataModel> {cart1, cart2});
        
        cart1 = await cartDao.FindByIdAsync(id1);
        cart2 = await cartDao.FindByIdAsync(id2);
        
        cart1.Should().NotBeNull();
        cart1!.CustomerId.Should().Be(null);
        
        cart2.Should().NotBeNull();
        cart2!.CustomerId.Should().Be(customerId1);
    }
    
    [Fact]
    public async Task BulkDeleteDeletesExistingEntities() {
        var cartDao = GetCartDao(ShopTenantId);
        var id1 = Guid.Parse("b4cf2977-13fc-44dd-89cf-0dbfdae04fce");
        var id2 = Guid.Parse("4c99b9b8-9cb5-4a49-ada9-01bd95f398d4");
        var nonExistingId = Guid.Parse("aef11853-bd61-4517-9907-ca51a50b7225");

        var ids = new List<Guid> { id1, id2, nonExistingId };
        var carts = await cartDao.FindByIdsAsync(ids).ToListAsync();
        carts.Count.Should().Be(2);
        carts[0].Id.Should().Be(id1);
        carts[1].Id.Should().Be(id2);
        
        await cartDao.DeleteAsync(carts);
        carts = await cartDao.FindByIdsAsync(ids).ToListAsync();
        carts.Should().BeEmpty();
    }

    private IDao<CartDataModel> GetCartDao(string tenantId) => GetDao(new CartDataRecordMapper(), tenantId);
    private IDao<ProductCartDataModel> GetCartItemDao(string tenantId) => GetDao(new ProductCartDataRecordMapper(), tenantId);
}