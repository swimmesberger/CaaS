using System.Collections.Immutable;
using CaaS.Core.Base;
using CaaS.Core.Base.Exceptions;
using CaaS.Core.Base.Tenant;
using CaaS.Core.CartAggregate;
using CaaS.Core.CouponAggregate;
using CaaS.Core.CustomerAggregate;
using CaaS.Core.DiscountAggregate;
using CaaS.Core.DiscountAggregate.Base;
using CaaS.Core.DiscountAggregate.Impl;
using CaaS.Core.ProductAggregate;
using CaaS.Core.ShopAggregate;
using CaaS.Infrastructure.Base.Tenant;
using CaaS.Infrastructure.CartData;
using CaaS.Infrastructure.CouponData;
using CaaS.Infrastructure.CustomerData;
using CaaS.Infrastructure.DiscountData;
using CaaS.Infrastructure.ProductData;
using CaaS.Infrastructure.ShopData;
using CaaS.Test.Common;
using FluentAssertions.Common;
using Microsoft.Extensions.Options;

namespace CaaS.Test.ServiceTests; 

public class CartServiceTest {
    private static readonly Guid TestShopId = new Guid("1AF5037B-16A0-430A-8035-6BCD785CBFB6");
    private static readonly string TestShopName = "TestShop";
    private static readonly Guid TestShopAdminId = new Guid("B0077779-E33B-4DFF-938B-47BA4A10854B");
    private static readonly Guid ExistingCartId = new Guid("0EE2F24E-E2DF-4A0E-B055-C804A6672D44");
    private static readonly Guid ProductAId = new Guid("05B7F6AB-4409-4417-9F76-34035AC92AE9");
    private static readonly Guid ProductBId = new Guid("DD7E1EA1-6D85-4596-AADB-A4648F7DA31F");
    private static readonly Guid CouponIdA = new Guid("BB791EA1-3CA5-9097-DFDB-CF648F7DA31F");
    private static readonly Guid CouponIdB = new Guid("3EC1A6BE-BBE7-4F62-8C85-601031D644B6");
    private static readonly Guid CouponIdC = new Guid("B552FAF6-D8A8-4F20-9943-C6D19CBE89A6");
    private static readonly Guid CartItem1Id = new Guid("82E369D6-3CC2-4468-9621-783E376E7E72");

    [Fact]
    public async Task CreateCartOptimistic() {
        var cartService = CreateCartService(SystemClock.Instance);

        var newCartId = new Guid("397CB87F-9694-4EAD-A549-E3089D61B131");
        var curTime = SystemClock.GetNow();
        var createdCart = await cartService.AddProductToCartAsync(newCartId, ProductAId, 1);
        createdCart.Should().NotBeNull();
        createdCart.ShopId.Should().Be(TestShopId);
        createdCart.Items.Should().HaveCount(1);
        var productA = createdCart.Items.FirstOrDefault(i => i.Product.Id == ProductAId);
        productA.Should().NotBeNull();
        productA!.Amount.Should().Be(1);
        createdCart.LastAccess.Should().BeOnOrAfter(curTime);
    }
    
    [Fact]
    public async Task AddProductToCartWhenProductExistsOptimistic() {
        var cartService = CreateCartService(SystemClock.Instance);

        var curTime = SystemClock.GetNow();
        var updatedCart = await cartService.AddProductToCartAsync(ExistingCartId, ProductAId, 2);
        updatedCart.Should().NotBeNull();
        updatedCart.ShopId.Should().Be(TestShopId);
        updatedCart.Items.Should().HaveCount(1);
        var productA = updatedCart.Items.FirstOrDefault(i => i.Product.Id == ProductAId);
        productA.Should().NotBeNull();
        productA!.Amount.Should().Be(4);
        updatedCart.LastAccess.Should().BeOnOrAfter(curTime);
    }
    
    [Fact]
    public async Task AddProductToCartWhenProductNotExistingOptimistic() {
        var cartService = CreateCartService(SystemClock.Instance);

        var curTime = SystemClock.GetNow();
        var updatedCart = await cartService.AddProductToCartAsync(ExistingCartId, ProductBId, 2);
        updatedCart.Should().NotBeNull();
        updatedCart.ShopId.Should().Be(TestShopId);
        updatedCart.Items.Should().HaveCount(2);
        var productA = updatedCart.Items.FirstOrDefault(i => i.Product.Id == ProductAId);
        productA.Should().NotBeNull();
        productA!.Amount.Should().Be(2);
        var productB = updatedCart.Items.FirstOrDefault(i => i.Product.Id == ProductBId);
        productB.Should().NotBeNull();
        productB!.Amount.Should().Be(2);
        updatedCart.LastAccess.Should().BeOnOrAfter(curTime);
    }
    
    [Fact]
    public async Task RemoveProductFromCartExistingOptimistic() {
        var cartService = CreateCartService(SystemClock.Instance);

        var curTime = SystemClock.GetNow();
        var updatedCart = await cartService.RemoveProductFromCartAsync(ExistingCartId, ProductAId);
        updatedCart.Should().NotBeNull();
        updatedCart.ShopId.Should().Be(TestShopId);
        updatedCart.Items.Should().BeEmpty();
        updatedCart.LastAccess.Should().BeOnOrAfter(curTime);
    }

    [Fact]
    public async Task DeleteExpiredCarts() {
        var currentTime = AsUtc(new DateTime(2022, 12, 18, 20, 0, 0, DateTimeKind.Local));
        var staticClock = new StaticSystemClock(currentTime);
        var cartService = CreateCartService(staticClock);
        var countBefore = await cartService.CountAsync();
        countBefore.Should().Be(3);
        await cartService.DeleteExpiredCartsAsync();
        var countAfter = await cartService.CountAsync();
        countAfter.Should().Be(1);
    }

    [Fact]
    public async Task AddCouponToCart() {
        var currentTime = AsUtc(new DateTime(2022, 12, 18, 20, 0, 0, DateTimeKind.Local));
        var staticClock = new StaticSystemClock(currentTime);
        var cartService = CreateCartService(staticClock);
        var cart = await cartService.GetByIdAsync(ExistingCartId);
        cart!.Coupons.Count().Should().Be(1);
        cart = await cartService.AddCouponToCartAsync(ExistingCartId, CouponIdB);
        cart!.Coupons.Count().Should().Be(2);
    }
    
    [Fact]
    public async Task AddCouponToPessimistic() {
        var currentTime = AsUtc(new DateTime(2022, 12, 18, 20, 0, 0, DateTimeKind.Local));
        var staticClock = new StaticSystemClock(currentTime);
        var cartService = CreateCartService(staticClock);
        var cart = await cartService.GetByIdAsync(ExistingCartId);
        cart!.Coupons.Count().Should().Be(1);
        Func<Task> act = async () => { await cartService.AddCouponToCartAsync(ExistingCartId, CouponIdC); };
        await act.Should().ThrowAsync<CaasValidationException>();
    }

    [Fact]
    public async Task AddProductQuantityOptimistic() {
        var currentTime = AsUtc(new DateTime(2022, 12, 18, 20, 0, 0, DateTimeKind.Local));
        var staticClock = new StaticSystemClock(currentTime);
        var cartService = CreateCartService(staticClock);
        var cart = await cartService.GetByIdAsync(ExistingCartId);
        cart!.Items.Count().Should().Be(1);
        cart.Items[0].Amount.Should().Be(2);
        
        cart = await cartService.SetProductQuantityInCartAsync(ExistingCartId, ProductAId, 7);
        cart.Items[0].Amount.Should().Be(7);
    }
    
    [Fact]
    public async Task AddProductQuantityInvalidQuantity() {
        var currentTime = AsUtc(new DateTime(2022, 12, 18, 20, 0, 0, DateTimeKind.Local));
        var staticClock = new StaticSystemClock(currentTime);
        var cartService = CreateCartService(staticClock);
        var cart = await cartService.GetByIdAsync(ExistingCartId);
        cart!.Items.Count().Should().Be(1);
        cart.Items[0].Amount.Should().Be(2);

        Func<Task> act = async () => { await cartService.SetProductQuantityInCartAsync(ExistingCartId, ProductAId, -1); };
        await act.Should().ThrowAsync<CaasValidationException>();
    }
    
    [Fact]
    public async Task AddProductQuantityInvalidCart() {
        var currentTime = AsUtc(new DateTime(2022, 12, 18, 20, 0, 0, DateTimeKind.Local));
        var staticClock = new StaticSystemClock(currentTime);
        var cartService = CreateCartService(staticClock);
        var cart = await cartService.GetByIdAsync(ExistingCartId);
        cart!.Items.Count().Should().Be(1);
        cart.Items[0].Amount.Should().Be(2);

        Func<Task> act = async () => { await cartService.SetProductQuantityInCartAsync(CouponIdA, ProductAId, 5); };
        await act.Should().ThrowAsync<CaasItemNotFoundException>();
    }
    
    [Fact]
    public async Task AddProductQuantityItemNotFound() {
        var currentTime = AsUtc(new DateTime(2022, 12, 18, 20, 0, 0, DateTimeKind.Local));
        var staticClock = new StaticSystemClock(currentTime);
        var cartService = CreateCartService(staticClock);
        var cart = await cartService.GetByIdAsync(ExistingCartId);
        cart!.Items.Count().Should().Be(1);
        cart.Items[0].Amount.Should().Be(2);

        Func<Task> act = async () => { await cartService.SetProductQuantityInCartAsync(ExistingCartId, ProductBId, 5); };
        await act.Should().ThrowAsync<CaasItemNotFoundException>();
    }


    private ICartService CreateCartService(ISystemClock clock) {
        var shopDao = new MemoryDao<ShopDataModel>(new List<ShopDataModel>() {
            new ShopDataModel() { Id = TestShopId, Name = TestShopName, AdminId = TestShopAdminId, CartLifetimeMinutes = 60}
        });
        var shopAdminDao = new MemoryDao<ShopAdminDataModel>(new List<ShopAdminDataModel>() {
            new ShopAdminDataModel() { Id = TestShopAdminId, ShopId = TestShopId }
        });
        var productDao = new MemoryDao<ProductDataModel>(new List<ProductDataModel>() {
            new ProductDataModel() { Id = ProductAId, Name = "ProductA", ShopId = TestShopId, Price = 20},
            new ProductDataModel() { Id = ProductBId, Name = "ProductB", ShopId = TestShopId, Price = 5 }
        });
        var customerDao = new MemoryDao<CustomerDataModel>(new List<CustomerDataModel>());
        var cartDao = new MemoryDao<CartDataModel>(new List<CartDataModel>() {
            new CartDataModel() { Id = ExistingCartId, ShopId = TestShopId, LastAccess = DateTimeOffset.Parse("2022-12-18 19:00:00 +00:00")},
            new CartDataModel() {ShopId = TestShopId, LastAccess = DateTimeOffset.Parse("2022-12-18 16:00:00 +00:00")},
            new CartDataModel() {ShopId = TestShopId, LastAccess = DateTimeOffset.Parse("2022-12-18 15:00:00 +00:00")},
        });
        var cartItemDao = new MemoryDao<ProductCartDataModel>(new List<ProductCartDataModel>() {
            new ProductCartDataModel() { Id = CartItem1Id, Amount = 2, CartId = ExistingCartId, ProductId = ProductAId }
        });
        var couponDao = new MemoryDao<CouponDataModel>(new List<CouponDataModel>() {
            new CouponDataModel { Id = CouponIdA, ShopId = TestShopId, Value = 4, OrderId = null, CartId = ExistingCartId, CustomerId = null},
            new CouponDataModel { Id = CouponIdB, ShopId = TestShopId, Value = 2, OrderId = null, CartId = null, CustomerId = null},
            new CouponDataModel { Id = CouponIdC, ShopId = TestShopId, Value = 1000, OrderId = null, CartId = null, CustomerId = null}
        });
        var discountSettingsDao = new MemoryDao<DiscountSettingDataModel>(new List<DiscountSettingDataModel>());
        
        var shopRepository = new ShopRepository(shopDao, shopAdminDao);
        var productRepository = new ProductRepository(productDao, shopRepository);
        var customerRepository = new CustomerRepository(customerDao);
        var couponRepository = new CouponRepository(couponDao);

        var tenantIdAccessor = new StaticTenantIdAccessor(TestShopId.ToString());
        var cartRepository = new CartRepository(cartDao, cartItemDao, productRepository, customerRepository,  couponRepository, clock);
        var componentFactory = new DiscountComponentFactory(ImmutableArray<DiscountComponentMetadata>.Empty, null!);
        var jsonConverter = new DiscountSettingRawConverter(new OptionsWrapper<DiscountJsonOptions>(new DiscountJsonOptions()), componentFactory.GetDiscountMetadata());
        var discountSettingsRepository = new DiscountSettingsRepository(discountSettingsDao, jsonConverter);
        var validator = new MockValidator();
        var discountService = new DiscountService(discountSettingsRepository, componentFactory, tenantIdAccessor, jsonConverter, validator);
        
        return new CartService(cartRepository, customerRepository, productRepository, shopRepository, discountService, couponRepository,
            tenantIdAccessor, clock);
    }

    private static DateTimeOffset AsUtc(DateTime dateTime) => dateTime.ToUniversalTime().ToDateTimeOffset();

}