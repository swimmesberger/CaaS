using CaaS.Core.Services;
using CaaS.Infrastructure.DataModel;
using CaaS.Infrastructure.Repositories;
using CaaS.Test.Base;
using CaaS.Test.Integration;

namespace CaaS.Test; 

public class CartServiceTest {
    private static readonly Guid TestShopId = new Guid("1AF5037B-16A0-430A-8035-6BCD785CBFB6");
    private const string TestShopName = "TestShop";
    private static readonly Guid ExistingCartId = new Guid("0EE2F24E-E2DF-4A0E-B055-C804A6672D44");
    private static readonly Guid ProductAId = new Guid("05B7F6AB-4409-4417-9F76-34035AC92AE9");
    private static readonly Guid ProductBId = new Guid("DD7E1EA1-6D85-4596-AADB-A4648F7DA31F");

    [Fact]
    public async Task CreateCartOptimistic() {
        var cartService = CreateCartService();

        var curTime = DateTimeOffset.UtcNow;
        var createdCart = await cartService.CreateCart();
        createdCart.Should().NotBeNull();
        createdCart.ShopId.Should().Be(TestShopId);
        createdCart.Items.Should().BeEmpty();
        createdCart.LastAccess.Should().BeAfter(curTime);
    }
    
    [Fact]
    public async Task AddProductToCartExistingOptimistic() {
        var cartService = CreateCartService();

        var curTime = DateTimeOffset.UtcNow;
        var updatedCart = await cartService.AddProductToCart(ExistingCartId, ProductAId, 2);
        updatedCart.Should().NotBeNull();
        updatedCart.ShopId.Should().Be(TestShopId);
        updatedCart.Items.Should().HaveCount(1);
        updatedCart.Items[0].Product.Id.Should().Be(ProductAId);
        updatedCart.Items[0].Amount.Should().Be(4);
        updatedCart.LastAccess.Should().BeAfter(curTime);
    }

    private ICartService CreateCartService() {
        var testShopAdminId = new Guid("B0077779-E33B-4DFF-938B-47BA4A10854B");


        var shopDao = new MemoryDao<ShopDataModel>(new List<ShopDataModel>() {
            new ShopDataModel() { Id = TestShopId, Name = TestShopName, AdminId = testShopAdminId }
        });
        var shopAdminDao = new MemoryDao<ShopAdminDataModel>(new List<ShopAdminDataModel>() {
            new ShopAdminDataModel() { Id = testShopAdminId, ShopId = TestShopId }
        });
        var productDao = new MemoryDao<ProductDataModel>(new List<ProductDataModel>() {
            new ProductDataModel() { Id = ProductAId, Name = "ProductA", ShopId = TestShopId },
            new ProductDataModel() { Id = ProductBId, Name = "ProductB", ShopId = TestShopId }
        });
        var customerDao = new MemoryDao<CustomerDataModel>(new List<CustomerDataModel>());
        var cartDao = new MemoryDao<CartDataModel>(new List<CartDataModel>() {
            new CartDataModel() { Id = ExistingCartId, ShopId = TestShopId }
        });
        var cartItemDao = new MemoryDao<ProductCartDataModel>(new List<ProductCartDataModel>() {
            new ProductCartDataModel() { Amount = 2, CartId = ExistingCartId, ProductId = ProductAId }
        });
        
        var shopRepository = new ShopRepository(shopDao, shopAdminDao);
        var productRepository = new ProductRepository(productDao, shopRepository);
        var customerRepository = new CustomerRepository(customerDao);

        var tenantIdAccessor = new StaticTenantIdAccessor(TestShopId.ToString());
        var cartRepository = new CartRepository(cartDao, cartItemDao, shopRepository, productRepository, customerRepository);
        return new DefaultCartService(cartRepository, tenantIdAccessor);
    }
}