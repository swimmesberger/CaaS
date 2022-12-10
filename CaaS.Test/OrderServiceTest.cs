using System.Collections.Immutable;
using CaaS.Core.Base;
using CaaS.Core.Base.Exceptions;
using CaaS.Core.CartAggregate;
using CaaS.Core.CouponAggregate;
using CaaS.Core.CustomerAggregate;
using CaaS.Core.DiscountAggregate;
using CaaS.Core.DiscountAggregate.Base;
using CaaS.Core.DiscountAggregate.Impl;
using CaaS.Core.OrderAggregate;
using CaaS.Infrastructure.Base.Tenant;
using CaaS.Infrastructure.CartData;
using CaaS.Infrastructure.CouponData;
using CaaS.Infrastructure.CustomerData;
using CaaS.Infrastructure.DiscountData;
using CaaS.Infrastructure.OrderData;
using CaaS.Infrastructure.ProductData;
using CaaS.Infrastructure.ShopData;
using CaaS.Test.Common;
using Microsoft.Extensions.Options;
using Moq;
using SwKo.Pay;
using SwKo.Pay.Api;
using SwKo.Pay.Api.Exceptions;
using Customer = CaaS.Core.CustomerAggregate.Customer;

namespace CaaS.Test; 

public class OrderServiceTest {
    private static readonly Guid TestShopId = new Guid("1AF5037B-16A0-430A-8035-6BCD785CBFB6");
    private const string TestShopName = "TestShop";
    private static readonly Guid ExistingOrderId = new Guid("0EE2F24E-E2DF-4A0E-B055-C804A6672D44");
    private static readonly Guid ExistingCartId1 = new Guid("5552F24E-E2DF-4A0E-B055-C804A6672D77");
    private static readonly Guid ExistingCartId2 = new Guid("E7E6112A-242B-45EA-9643-A4757360C175");
    private static readonly Guid ExistingCartId3 = new Guid("B5421920-C375-469A-856D-6D22A1F90412");
    private static readonly Guid ExistingCartId4 = new Guid("96CBC24D-B412-4342-8134-9CAAC9FF6EE9");
    private static readonly Guid ExistingCartId5 = new Guid("C0ABC5C8-1467-4E39-A888-3449F774BCEE");
    private static readonly Guid CustomerIdA = new Guid("99C91EA1-4CA5-9097-DFDB-CF688F0DA31F");
    private static readonly Guid CustomerIdB = new Guid("C162AF5D-7474-4A96-B830-E3D4D57B4170");
    private static readonly Guid CustomerIdC = new Guid("5B7816F2-E174-49CE-85B2-94BB36534E35");
    private static readonly Guid CustomerIdD = new Guid("84722277-4AA6-4021-9FD0-7D1F4FD7371E");
    private static readonly Guid ProductAId = new Guid("05B7F6AB-4409-4417-9F76-34035AC92AE9");
    private static readonly Guid ProductBId = new Guid("DD7E1EA1-6D85-4596-AADB-A4648F7DA31F");
    private static readonly Guid CartItemIdA = new Guid("37D2E80C-CC04-4369-83E3-EF278CCB956D");
    private static readonly Guid CartItemIdB = new Guid("614A8687-1F49-41C4-8B7D-2614F88E5F76");

    private static readonly Address Address = new() {
        Street = "Street",
        City = "City",
        State = "State",
        Country = "Country",
        ZipCode = "ZipCode"
    };
    
    [Fact]
    public async Task CreateOrderOptimistic() {
        var paymentService = new PaymentServiceImpl();
        var orderService = CreateOrderService(paymentService);
        
        var createOrder = await orderService.CreateOrder(CustomerIdA, Address);
        createOrder.Should().NotBeNull();
        createOrder.ShopId.Should().Be(TestShopId);
        createOrder.Items.Should().BeEmpty();
    }
    
    [Fact]
    public async Task FindOrderByIdOptimistic() {
        var paymentService = new PaymentServiceImpl();
        var orderService = CreateOrderService(paymentService);

        var foundOrder = await orderService.FindOrderById(ExistingOrderId);
        foundOrder.Should().NotBeNull();
        foundOrder!.Id.Should().Be(ExistingOrderId);
        foundOrder.Items.Count.Should().Be(2);
    }
    
    [Fact]
    public async Task CreateOrderFromCartWhenCartNotFound() {
        var paymentService = new PaymentServiceImpl();
        var orderService = CreateOrderService(paymentService);

        Func<Task> act = async () => { await orderService.CreateOrderFromCart(Guid.Parse("56C5AA6D-A4D6-48B1-9973-51CAC50204A3"), Address); };
        await act.Should().ThrowAsync<CaasItemNotFoundException>();
    }

    [Fact]
    public async Task CreateOrderFromCartOptimisticCase() {
        var paymentService = new PaymentServiceImpl();
        var orderService = CreateOrderService(paymentService);
        
        var result = await orderService.CreateOrderFromCart(ExistingCartId1, Address);
        result.Items.Count.Should().Be(2);
        result.Items[0].Product.Id.Should().Be(ProductAId);
        result.Items[0].Amount.Should().Be(2);
        result.Items[0].OrderItemDiscounts.Count.Should().Be(0);
        result.Coupons.Count.Should().Be(2);
        result.Coupons[0].OrderId.Should().Be(result.Id);
        result.Coupons[0].CartId.Should().Be(null);
    }

    [Fact]
    public async Task CreateOrderFromCartWhenCreditCardIsInvalid() {
        var paymentService = new PaymentServiceImpl();
        var orderService = CreateOrderService(paymentService);
        
        Func<Task> act = async () => { await orderService.CreateOrderFromCart(ExistingCartId2, Address); };
        await act.Should().ThrowAsync<SwKoPayCreditCardInvalidException>();
    }

    [Fact]
    public async Task CreateOrderFromCartWhenCreditCardIsInactive() {
        var paymentService = new PaymentServiceImpl();
        var orderService = CreateOrderService(paymentService);
        
        Func<Task> act = async () => { await orderService.CreateOrderFromCart(ExistingCartId3, Address); };
        await act.Should().ThrowAsync<SwKoPayCardInactiveException>();
    }
    
    [Fact]
    public async Task CreateOrderFromCartWhenCreditIsInsufficient() {
        var paymentService = new PaymentServiceImpl();
        var orderService = CreateOrderService(paymentService);
        
        Func<Task> act = async () => { await orderService.CreateOrderFromCart(ExistingCartId4, Address); };
        await act.Should().ThrowAsync<SwKoPayInsufficientCreditException>();
    }

    [Fact]
    public async Task CreateOrderFromCartWhenCaasDbExceptionIsThrown() {
        var orderRepositoryMock = new Mock<IOrderRepository>();
        var customerRepositoryMock = new Mock<ICustomerRepository>();
        var cartServiceMock = new Mock<ICartService>();
        var couponRepositoryMock = new Mock<ICouponRepository>();
        var tenantIdAccessor = new StaticTenantIdAccessor(TestShopId.ToString());
        var uowManager = new MockUnitOfWorkManager();
        var paymentService = new Mock<IPaymentService>();

        Customer customer = new Customer() {
            Id = CustomerIdA,
            ShopId = TestShopId,
            EMail = "test@test.com",
            Name = "Roman",
            CreditCardNumber = "4405100664466647"
        };

        customerRepositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync(new Customer() { Id = CustomerIdA });

        cartServiceMock.Setup(x => x.GetCartById(It.IsAny<Guid>(), CancellationToken.None)).
            ReturnsAsync(new Cart(){Id = ExistingCartId1, Customer = customer});
        
        couponRepositoryMock.Setup(x => 
                x.UpdateAsync(It.IsAny<IEnumerable<Coupon>>(), It.IsAny<IEnumerable<Coupon>>(), CancellationToken.None)).
            Throws(new CaasDbException("caas db exception"));
        
        var orderService = new OrderService(orderRepositoryMock.Object, customerRepositoryMock.Object, cartServiceMock.Object, couponRepositoryMock.Object, 
            tenantIdAccessor, uowManager, paymentService.Object);

        Func<Task> act = async () => { await orderService.CreateOrderFromCart(ExistingCartId1, Address);};
        await act.Should().ThrowAsync<CaasDbException>();
        
        paymentService.Verify(impl => impl.RefundPayment(It.IsAny<Guid>()), Times.Exactly(1));
    }
    

    private IOrderService CreateOrderService(IPaymentService paymentService) {
        var testShopAdminId = new Guid("B0077779-E33B-4DFF-938B-47BA4A10854B");
        
        var shopDao = new MemoryDao<ShopDataModel>(new List<ShopDataModel>() {
            new ShopDataModel() { Id = TestShopId, Name = TestShopName, AdminId = testShopAdminId }
        });
        var shopAdminDao = new MemoryDao<ShopAdminDataModel>(new List<ShopAdminDataModel>() {
            new ShopAdminDataModel() { Id = testShopAdminId, ShopId = TestShopId }
        });
        var productDao = new MemoryDao<ProductDataModel>(new List<ProductDataModel>() {
            new ProductDataModel() { Id = ProductAId, Name = "ProductA", Price = 10, ShopId = TestShopId },
            new ProductDataModel() { Id = ProductBId, Name = "ProductB", Price = 20, ShopId = TestShopId }
        });
        var customerDao = new MemoryDao<CustomerDataModel>(new List<CustomerDataModel>() {
            new CustomerDataModel { Id = CustomerIdA, ShopId = TestShopId, Name = "Roman Koho", EMail = "roman@test.com", CreditCardNumber = "4405100664466647" },
            new CustomerDataModel { Id = CustomerIdB, ShopId = TestShopId, Name = "User 2", EMail = "user2@test.com", CreditCardNumber = "9999999999999999" },
            new CustomerDataModel { Id = CustomerIdC, ShopId = TestShopId, Name = "User with invalid credit card", EMail = "invalid@test.com", CreditCardNumber = "4594233824721535" },
            new CustomerDataModel { Id = CustomerIdD, ShopId = TestShopId, Name = "User with no money", EMail = "poor@test.com", CreditCardNumber = "5349801875979163" },
        });
        
        var orderDao = new MemoryDao<OrderDataModel>(new List<OrderDataModel>() {
            new OrderDataModel { Id = ExistingOrderId, ShopId = TestShopId, OrderNumber = 54212, CustomerId = CustomerIdA, OrderDate = DateTimeOffsetProvider.GetNow() }
        });
        
        var orderItemDao = new MemoryDao<ProductOrderDataModel>(new List<ProductOrderDataModel>() {
            new ProductOrderDataModel() { Amount = 2, OrderId = ExistingOrderId, ProductId = ProductAId },
            new ProductOrderDataModel() { Amount = 5, OrderId = ExistingOrderId, ProductId = ProductAId }
        });

        var orderDiscountDao = new MemoryDao<OrderDiscountDataModel>(new List<OrderDiscountDataModel>() {
            new OrderDiscountDataModel() { Id = Guid.NewGuid(), DiscountName = "Christmas", Discount = 5, OrderId = ExistingOrderId, ShopId = TestShopId}
        });

        var couponDao = new MemoryDao<CouponDataModel>(new List<CouponDataModel>() {
            new CouponDataModel { Id = Guid.NewGuid(), ShopId = TestShopId, Value = 4, OrderId = null, CartId = ExistingCartId1, CustomerId = CustomerIdA},
            new CouponDataModel { Id = Guid.NewGuid(), ShopId = TestShopId, Value = 2, OrderId = null, CartId = ExistingCartId1, CustomerId = CustomerIdA}
        });

        var orderItemDiscountDao = new MemoryDao<ProductOrderDiscountDataModel>(new List<ProductOrderDiscountDataModel>() {
            new ProductOrderDiscountDataModel { Id = Guid.NewGuid(), DiscountName = "10% on most expensive product", 
                Discount = (decimal)1.10, ProductOrderId = default, ShopId = TestShopId}
        });
        
        var cartDao = new MemoryDao<CartDataModel>(new List<CartDataModel>() {
            new CartDataModel() { Id = ExistingCartId1, ShopId = TestShopId, CustomerId = CustomerIdA},
            new CartDataModel() { Id = ExistingCartId2, ShopId = TestShopId, CustomerId = CustomerIdB},
            new CartDataModel() { Id = ExistingCartId3, ShopId = TestShopId, CustomerId = CustomerIdC},
            new CartDataModel() { Id = ExistingCartId4, ShopId = TestShopId, CustomerId = CustomerIdD},
            new CartDataModel() { Id = ExistingCartId5, ShopId = TestShopId, CustomerId = null},
            
        });
        var cartItemDao = new MemoryDao<ProductCartDataModel>(new List<ProductCartDataModel>() {
            new ProductCartDataModel() {Id = CartItemIdA, Amount = 2, CartId = ExistingCartId1, ProductId = ProductAId},
            new ProductCartDataModel() {Id = CartItemIdB, Amount = 4, CartId = ExistingCartId1, ProductId = ProductBId},
            new ProductCartDataModel() {Id = Guid.NewGuid(), Amount = 5, CartId = ExistingCartId2, ProductId = ProductBId},
            new ProductCartDataModel() {Id = Guid.NewGuid(), Amount = 2, CartId = ExistingCartId2, ProductId = ProductBId},
            new ProductCartDataModel() {Id = Guid.NewGuid(), Amount = 2, CartId = ExistingCartId4, ProductId = ProductBId},
            new ProductCartDataModel() {Id = Guid.NewGuid(), Amount = 2, CartId = ExistingCartId4, ProductId = ProductBId}
        });
        
        var shopRepository = new ShopRepository(shopDao, shopAdminDao);
        var productRepository = new ProductRepository(productDao, shopRepository);
        var customerRepository = new CustomerRepository(customerDao);
        var couponRepository = new CouponRepository(couponDao);
        
        var cartRepository = new CartRepository(cartDao, cartItemDao, productRepository, customerRepository, 
            couponRepository, DateTimeOffsetProvider.Instance);

        var tenantIdAccessor = new StaticTenantIdAccessor(TestShopId.ToString());
        var orderRepository = new OrderRepository(orderDao, orderItemDao, orderItemDiscountDao, orderDiscountDao, 
                                                    productRepository, customerRepository, couponRepository);
        
        var discountSettingsDao = new MemoryDao<DiscountSettingDataModel>(new List<DiscountSettingDataModel>());
        var componentFactory = new DiscountComponentFactory(ImmutableArray<DiscountComponentMetadata>.Empty, null!);
        var jsonConverter = new DiscountSettingRawConverter(new OptionsWrapper<DiscountJsonOptions>(new DiscountJsonOptions()), componentFactory.GetDiscountMetadata());
        var validator = new MockValidator();
        var discountSettingsRepository = new DiscountSettingsRepository(discountSettingsDao, jsonConverter);
        var discountService = new CaasDiscountService(discountSettingsRepository, componentFactory, tenantIdAccessor, jsonConverter, validator);

        var uowManager = new MockUnitOfWorkManager();

        var cartService = new CartService(cartRepository, customerRepository, productRepository, shopRepository, discountService, couponRepository,
            tenantIdAccessor, DateTimeOffsetProvider.Instance);
        return new OrderService(orderRepository, customerRepository, cartService, couponRepository, tenantIdAccessor, uowManager, paymentService);
    }
    
}