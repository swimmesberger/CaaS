﻿using System.Collections.Immutable;
using CaaS.Core.Base;
using CaaS.Core.CartAggregate;
using CaaS.Core.DiscountAggregate;
using CaaS.Core.DiscountAggregate.Base;
using CaaS.Core.DiscountAggregate.Impl;
using CaaS.Infrastructure.Base.Tenant;
using CaaS.Infrastructure.CartData;
using CaaS.Infrastructure.CouponData;
using CaaS.Infrastructure.CustomerData;
using CaaS.Infrastructure.DiscountData;
using CaaS.Infrastructure.ProductData;
using CaaS.Infrastructure.ShopData;
using CaaS.Test.Common;
using Microsoft.Extensions.Options;

namespace CaaS.Test.ServiceTests; 

public class CartServiceTest {
    private static readonly Guid TestShopId = new Guid("1AF5037B-16A0-430A-8035-6BCD785CBFB6");
    private const string TestShopName = "TestShop";
    private static readonly Guid ExistingCartId = new Guid("0EE2F24E-E2DF-4A0E-B055-C804A6672D44");
    private static readonly Guid ProductAId = new Guid("05B7F6AB-4409-4417-9F76-34035AC92AE9");
    private static readonly Guid ProductBId = new Guid("DD7E1EA1-6D85-4596-AADB-A4648F7DA31F");
    private static readonly Guid CouponIdA = new Guid("BB791EA1-3CA5-9097-DFDB-CF648F7DA31F");
    private static readonly Guid CouponIdB = new Guid("3EC1A6BE-BBE7-4F62-8C85-601031D644B6");

    [Fact]
    public async Task CreateCartOptimistic() {
        var cartService = CreateCartService();

        var newCartId = new Guid("397CB87F-9694-4EAD-A549-E3089D61B131");
        var curTime = SystemClock.GetNow();
        var createdCart = await cartService.AddProductToCart(newCartId, ProductAId, 1);
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
        var cartService = CreateCartService();

        var curTime = SystemClock.GetNow();
        var updatedCart = await cartService.AddProductToCart(ExistingCartId, ProductAId, 2);
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
        var cartService = CreateCartService();

        var curTime = SystemClock.GetNow();
        var updatedCart = await cartService.AddProductToCart(ExistingCartId, ProductBId, 2);
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
        var cartService = CreateCartService();

        var curTime = SystemClock.GetNow();
        var updatedCart = await cartService.RemoveProductFromCart(ExistingCartId, ProductAId);
        updatedCart.Should().NotBeNull();
        updatedCart.ShopId.Should().Be(TestShopId);
        updatedCart.Items.Should().BeEmpty();
        updatedCart.LastAccess.Should().BeOnOrAfter(curTime);
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
        var couponDao = new MemoryDao<CouponDataModel>(new List<CouponDataModel>() {
            new CouponDataModel { Id = CouponIdA, ShopId = TestShopId, Value = 4, OrderId = null, CartId = ExistingCartId, CustomerId = null},
            new CouponDataModel { Id = CouponIdB, ShopId = TestShopId, Value = 2, OrderId = null, CartId = ExistingCartId, CustomerId = null}
        });
        var discountSettingsDao = new MemoryDao<DiscountSettingDataModel>(new List<DiscountSettingDataModel>());
        
        var shopRepository = new ShopRepository(shopDao, shopAdminDao);
        var productRepository = new ProductRepository(productDao, shopRepository);
        var customerRepository = new CustomerRepository(customerDao);
        var couponRepository = new CouponRepository(couponDao);

        var tenantIdAccessor = new StaticTenantIdAccessor(TestShopId.ToString());
        var cartRepository = new CartRepository(cartDao, cartItemDao, productRepository, customerRepository,  couponRepository, SystemClock.Instance);
        var componentFactory = new DiscountComponentFactory(ImmutableArray<DiscountComponentMetadata>.Empty, null!);
        var jsonConverter = new DiscountSettingRawConverter(new OptionsWrapper<DiscountJsonOptions>(new DiscountJsonOptions()), componentFactory.GetDiscountMetadata());
        var discountSettingsRepository = new DiscountSettingsRepository(discountSettingsDao, jsonConverter);
        var validator = new MockValidator();
        var discountService = new CaasDiscountService(discountSettingsRepository, componentFactory, tenantIdAccessor, jsonConverter, validator);

        return new CartService(cartRepository, customerRepository, productRepository, shopRepository, discountService, couponRepository,
            tenantIdAccessor, SystemClock.Instance);
    }
}