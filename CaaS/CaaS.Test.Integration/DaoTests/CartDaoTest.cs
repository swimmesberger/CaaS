using CaaS.Core.Exceptions;
using CaaS.Infrastructure.Ado.Base;
using CaaS.Infrastructure.Ado.Model;
using CaaS.Infrastructure.DataModel;
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

    private IDao<CartDataModel> GetCartDao(string tenantId) => GetDao(new CartDataRecordMapper(), tenantId);
}