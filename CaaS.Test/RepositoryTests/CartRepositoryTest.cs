using CaaS.Core.CartAggregate;
using CaaS.Infrastructure.CartData;
using CaaS.Infrastructure.CouponData;
using CaaS.Infrastructure.CustomerData;
using CaaS.Infrastructure.ProductData;
using CaaS.Infrastructure.ShopData;
using CaaS.Test.Common;

namespace CaaS.Test.RepositoryTests; 

public class CartRepositoryTest {
    private static readonly Guid TestShopId = new Guid("1AF5037B-16A0-430A-8035-6BCD785CBFB6");
    private static readonly string TestShopName = "TestShop";
    private static readonly Guid ExistingCart1Id = new Guid("0EE2F24E-E2DF-4A0E-B055-C804A6672D44");
    private static readonly Guid ExistingCart2Id = new Guid("AAABBB4E-E2DF-4A0E-B055-C804A6672555");
    private static readonly Guid CustomerIdA = new Guid("99C91EA1-4CA5-9097-DFDB-CF688F0DA31F");
    private static readonly Guid CustomerIdB = new Guid("9C0E8AA6-94C6-43F2-8446-D496DED2D7FE");
    private static readonly Guid ProductAId = new Guid("05B7F6AB-4409-4417-9F76-34035AC92AE9");
    private static readonly Guid ProductBId = new Guid("DD7E1EA1-6D85-4596-AADB-A4648F7DA31F");
    private static readonly Guid CouponIdA = new Guid("BB791EA1-3CA5-9097-DFDB-CF648F7DA31F");
    private static readonly Guid CartItemAId = new Guid("DA191BA1-3CA5-9097-DFDB-CF648F7DA31F");
    private static readonly Guid CartItemBId = new Guid("F113EA14-3CA5-9097-DFDB-CF648F7DA31F");
    private static readonly Guid CartItemCId = new Guid("4046B163-DA7B-42BF-8D1B-2FD0E655981B");
    private static readonly Guid CartItemDId = new Guid("D5B1092F-9C44-4FE8-8B24-9662F80702FE");
    private static readonly Guid TestShopAdminId = new Guid("B0077779-E33B-4DFF-938B-47BA4A10854B");

    [Fact]
    public async Task FindByCartIdOptimistic() {
        var currentTime = AsUtc(new DateTime(2022, 12, 18, 16, 30, 0, DateTimeKind.Local));
        var cartRepository = CreateCartRepository(currentTime);
        var result = await cartRepository.FindByIdAsync(ExistingCart1Id);
        result!.Id.Should().Be(ExistingCart1Id);
        result.Items.Count().Should().Be(2);
        result.Items[0].Id.Should().Be(CartItemAId);
    }

    [Fact]
    public async Task FindByCartIdPessimistic() {
        var currentTime = AsUtc(new DateTime(2022, 12, 18, 16, 30, 0, DateTimeKind.Local));
        var cartRepository = CreateCartRepository(currentTime);
        var result = await cartRepository.FindByIdAsync(CouponIdA);
        result.Should().BeNull();
    }

    [Fact]
    public async Task FindExpiredCarts() {
        var currentTime = AsUtc(new DateTime(2022, 12, 18, 16, 30, 0, DateTimeKind.Local));
        var cartRepository = CreateCartRepository(currentTime);
        var result = await cartRepository.FindExpiredCarts(120);
        result.Count().Should().Be(1);
        result[0].Id.Should().Be(ExistingCart2Id);
    }

    [Fact]
    public async Task DeleteSingle() {
        var currentTime = AsUtc(new DateTime(2022, 12, 18, 16, 30, 0, DateTimeKind.Local));
        var cartRepository = CreateCartRepository(currentTime);
        var cart = await cartRepository.FindByIdAsync(ExistingCart1Id);
        cart!.Id.Should().Be(ExistingCart1Id);
        await cartRepository.DeleteAsync(cart);
        cart = await cartRepository.FindByIdAsync(ExistingCart1Id);
        cart.Should().BeNull();
    }

    [Fact]
    public async Task BulkDelete() {
        var currentTime = AsUtc(new DateTime(2022, 12, 18, 16, 30, 0, DateTimeKind.Local));
        var cartRepository = CreateCartRepository(currentTime);
        var cart1 = await cartRepository.FindByIdAsync(ExistingCart1Id);
        var cart2 = await cartRepository.FindByIdAsync(ExistingCart2Id);
        var cartList = new List<Cart>() { cart1!, cart2! };
        await cartRepository.DeleteAsync(cartList);
        cart1 = await cartRepository.FindByIdAsync(ExistingCart1Id);
        cart2 = await cartRepository.FindByIdAsync(ExistingCart2Id);
        cart1.Should().BeNull();
        cart2.Should().BeNull();
    }

    [Fact]
    public async Task Count() {
        var currentTime = AsUtc(new DateTime(2022, 12, 18, 16, 30, 0, DateTimeKind.Local));
        var cartRepository = CreateCartRepository(currentTime);
        var count = await cartRepository.CountAsync();
        count.Should().Be(2);
    }
    
    private ICartRepository CreateCartRepository(DateTimeOffset currentDate) {
        var customerDao = new MemoryDao<CustomerDataModel>(new List<CustomerDataModel>() {
            new CustomerDataModel { Id = CustomerIdA, ShopId = TestShopId, Name = "Roman Koho", EMail = "test@test.com", CreditCardNumber = "1111222233334444" },
            new CustomerDataModel { Id = CustomerIdB, ShopId = TestShopId, Name = "Simon Wimmesb", EMail = "test@test.com", CreditCardNumber = "9999948945454" }
        });

        var shopDao = new MemoryDao<ShopDataModel>(new List<ShopDataModel>() {
            new ShopDataModel() { Id = TestShopId, Name = TestShopName, AdminId = TestShopAdminId, CartLifetimeMinutes = 120}
        });
        var shopAdminDao = new MemoryDao<ShopAdminDataModel>(new List<ShopAdminDataModel>() {
            new ShopAdminDataModel() { Id = TestShopAdminId, ShopId = TestShopId }
        });
        
        var productDao = new MemoryDao<ProductDataModel>(new List<ProductDataModel>() {
            new ProductDataModel() { Id = ProductAId, Name = "ProductA", Price = 10, ShopId = TestShopId },
            new ProductDataModel() { Id = ProductBId, Name = "ProductB", Price = 20, ShopId = TestShopId }
        });

        var cartItemDao = new MemoryDao<ProductCartDataModel>(new List<ProductCartDataModel>() {
            new ProductCartDataModel { Id = CartItemAId, ShopId = TestShopId, ProductId = ProductAId, CartId = ExistingCart1Id, Amount = 2 },
            new ProductCartDataModel { Id = CartItemBId, ShopId = TestShopId, ProductId = ProductBId, CartId = ExistingCart1Id, Amount = 3 },
            new ProductCartDataModel { Id = CartItemCId, ShopId = TestShopId, ProductId = ProductAId, CartId = ExistingCart2Id, Amount = 5 },
            new ProductCartDataModel { Id = CartItemDId, ShopId = TestShopId, ProductId = ProductBId, CartId = ExistingCart2Id, Amount = 8 }
        });
        
        var couponDao = new MemoryDao<CouponDataModel>(new List<CouponDataModel>() {
            new CouponDataModel { Id = CouponIdA, ShopId = TestShopId, Value = 4, OrderId = null, CartId = ExistingCart1Id, CustomerId = CustomerIdA}
        });

        var cartDao = new MemoryDao<CartDataModel>(new List<CartDataModel>() {
            new CartDataModel { Id = ExistingCart1Id, ShopId = TestShopId, CustomerId = CustomerIdA, LastAccess = DateTimeOffset.Parse("2022-12-18 16:00:00") },
            new CartDataModel { Id = ExistingCart2Id, ShopId = TestShopId, CustomerId = CustomerIdB, LastAccess = DateTimeOffset.Parse("2022-05-24 12:15:30") }
        });

        var shopRepository = new ShopRepository(shopDao, shopAdminDao);
        var productRepository = new ProductRepository(productDao, shopRepository);
        var customerRepository = new CustomerRepository(customerDao);
        var couponRepository = new CouponRepository(couponDao);
        
        return new CartRepository(cartDao, cartItemDao, productRepository, customerRepository, couponRepository, new StaticSystemClock(currentDate));
    }
    
    private static DateTimeOffset AsUtc(DateTime dateTime) => new DateTimeOffset(dateTime, TimeZoneInfo.Local.GetUtcOffset(dateTime));
}