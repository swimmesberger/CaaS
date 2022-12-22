using CaaS.Core.Base.Exceptions;
using CaaS.Core.CouponAggregate;
using CaaS.Infrastructure.Base.Tenant;
using CaaS.Infrastructure.CartData;
using CaaS.Infrastructure.CouponData;
using CaaS.Infrastructure.CustomerData;
using CaaS.Infrastructure.OrderData;
using CaaS.Infrastructure.ProductData;
using CaaS.Infrastructure.ShopData;
using CaaS.Test.Common;
using FluentAssertions.Common;

namespace CaaS.Test.ServiceTests; 

public class CouponServiceTest {
    private static readonly Guid TestShopId = new Guid("1AF5037B-16A0-430A-8035-6BCD785CBFB6");
    private static readonly string TestShopName = "TestShop";
    private static readonly Guid TestShopAdminId = new Guid("B0077779-E33B-4DFF-938B-47BA4A10854B");
    private static readonly Guid ExistingOrderId = new Guid("0EE2F24E-E2DF-4A0E-B055-C804A6672D44");
    private static readonly Guid ExistingOrder2Id = new Guid("AAABBB4E-E2DF-4A0E-B055-C804A6672555");
    private static readonly Guid ExistingCart1Id = new Guid("0EE2F24E-E2DF-4A0E-B055-C804A6672D44");
    private static readonly Guid ExistingCart2Id = new Guid("ADD15019-BAE3-4254-BE10-54157F852CCA");
    private static readonly Guid CustomerIdA = new Guid("99C91EA1-4CA5-9097-DFDB-CF688F0DA31F");
    private static readonly Guid CustomerIdB = new Guid("9C0E8AA6-94C6-43F2-8446-D496DED2D7FE");
    private static readonly Guid ProductAId = new Guid("05B7F6AB-4409-4417-9F76-34035AC92AE9");
    private static readonly Guid ProductBId = new Guid("DD7E1EA1-6D85-4596-AADB-A4648F7DA31F");
    private static readonly Guid CouponIdA = new Guid("BB791EA1-3CA5-9097-DFDB-CF648F7DA31F");
    private static readonly Guid CouponIdB = new Guid("4276016B-2BE5-41C7-994A-1486257605CB");
    private static readonly Guid CouponIdC = new Guid("B5215C84-7419-4D22-B224-F24F0E28767D");
    private static readonly Guid CouponIdD = new Guid("7986CE25-2E4F-4F44-8ABF-7FA68ADF152E");
    private static readonly Guid CouponIdE = new Guid("D7DDCD84-DF53-46A5-A143-B8DF8700A3E8");
    private static readonly Guid OrderItemAId = new Guid("DA191BA1-3CA5-9097-DFDB-CF648F7DA31F");
    private static readonly Guid OrderItemBId = new Guid("F113EA14-3CA5-9097-DFDB-CF648F7DA31F");
    private static readonly Guid OrderDiscountAId = new Guid("AA1B1EA1-6D85-4596-AADB-CF648F7DA31F");
    private static readonly Guid OrderDiscountBId = new Guid("BB1B1EA1-6D85-4596-AADB-CF648F7DA31F");
    private static readonly Guid OrderDiscountCId = new Guid("CC1B1EA1-6D85-4596-AADB-CF648F7DA31F");
    private static readonly Guid OrderItemDiscountA = new Guid("DE123EA1-3CA5-9097-DFDB-CF648F7DA31F");
    private static readonly Guid OrderItemDiscountB = new Guid("BB123EA1-3CA5-9097-DFDB-CF648F7DA31F");

    [Fact]
    public async Task GetByIdOptimistic() {
        var currentTime = AsUtc(new DateTime(2022, 11, 24, 0, 0, 0, DateTimeKind.Local));
        var couponService = CreateCouponService(currentTime);
        var result = await couponService.GetByIdAsync(CouponIdA);
        result.Id.Should().Be(CouponIdA);

        result = await couponService.GetByIdAsync(ExistingOrderId);
        result.Should().BeNull();
    }
    
    [Fact]
    public async Task GetByCartId() {
        var currentTime = AsUtc(new DateTime(2022, 11, 24, 0, 0, 0, DateTimeKind.Local));
        var couponService = CreateCouponService(currentTime);
        var result = await couponService.GetByCartIdAsync(ExistingCart1Id);
        result.Count().Should().Be(2);
        var items = result.ToArray();
        items[0].Id.Should().Be(CouponIdA);
        items[1].Id.Should().Be(CouponIdB);
    }
    
    [Fact]
    public async Task GetByCartIdPessimistic() {
        var currentTime = AsUtc(new DateTime(2022, 11, 24, 0, 0, 0, DateTimeKind.Local));
        var couponService = CreateCouponService(currentTime);
        var result = await couponService.GetByCartIdAsync(CustomerIdA);
        result.Count().Should().Be(0);
    }
    
    [Fact]
    public async Task GetByOrderId() {
        var currentTime = AsUtc(new DateTime(2022, 11, 24, 0, 0, 0, DateTimeKind.Local));
        var couponService = CreateCouponService(currentTime);
        var result = await couponService.GetByOrderIdAsync(ExistingOrderId);
        result.Count().Should().Be(2);
        var items = result.ToArray();
        items[0].Id.Should().Be(CouponIdC);
        items[1].Id.Should().Be(CouponIdD);
    }
    
    [Fact]
    public async Task GetByOrderIdPessimistic() {
        var currentTime = AsUtc(new DateTime(2022, 11, 24, 0, 0, 0, DateTimeKind.Local));
        var couponService = CreateCouponService(currentTime);
        var result = await couponService.GetByOrderIdAsync(CustomerIdA);
        result.Count().Should().Be(0);
    }
    
    [Fact]
    public async Task GetByCustomerId() {
        var currentTime = AsUtc(new DateTime(2022, 11, 24, 0, 0, 0, DateTimeKind.Local));
        var couponService = CreateCouponService(currentTime);
        var result = await couponService.GetByCustomerIdAsync(CustomerIdA);
        result.Count().Should().Be(2);
        var items = result.ToArray();
        items[0].Id.Should().Be(CouponIdA);
        items[1].Id.Should().Be(CouponIdB);
    }
    
    [Fact]
    public async Task GetByCustomerIdPessimistic() {
        var currentTime = AsUtc(new DateTime(2022, 11, 24, 0, 0, 0, DateTimeKind.Local));
        var couponService = CreateCouponService(currentTime);
        var result = await couponService.GetByCustomerIdAsync(ExistingCart1Id);
        result.Count().Should().Be(0);
    }

    [Fact]
    public async Task SetValueOptimistic() {
        var currentTime = AsUtc(new DateTime(2022, 11, 24, 0, 0, 0, DateTimeKind.Local));
        var couponService = CreateCouponService(currentTime);
        var coupon = await couponService.GetByIdAsync(CouponIdE);
        coupon.Value.Should().Be(40);

        coupon = await couponService.SetValueOfCouponAsync(CouponIdE, 100);
        coupon.Value.Should().Be(100);
    }
    
    [Fact]
    public async Task SetValueOfRedeemedCouponPessimistic() {
        var currentTime = AsUtc(new DateTime(2022, 11, 24, 0, 0, 0, DateTimeKind.Local));
        var couponService = CreateCouponService(currentTime);
        var coupon = await couponService.GetByIdAsync(CouponIdA);
        coupon.Value.Should().Be(4);

        Func<Task> act = async () => { await couponService.SetValueOfCouponAsync(CouponIdA, 20); };
        await act.Should().ThrowAsync<CaasValidationException>();
    }
    
    [Fact]
    public async Task SetValueOfNonExistingCouponPessimistic() {
        var currentTime = AsUtc(new DateTime(2022, 11, 24, 0, 0, 0, DateTimeKind.Local));
        var couponService = CreateCouponService(currentTime);
        var coupon = await couponService.GetByIdAsync(CouponIdA);
        coupon.Value.Should().Be(4);

        Func<Task> act = async () => { await couponService.SetValueOfCouponAsync(CustomerIdA, 20); };
        await act.Should().ThrowAsync<CaasItemNotFoundException>();
    }

    [Fact]
    public async Task UpdateOptimistic() {
        var currentTime = AsUtc(new DateTime(2022, 11, 24, 0, 0, 0, DateTimeKind.Local));
        var couponService = CreateCouponService(currentTime);
        var coupon = await couponService.GetByIdAsync(CouponIdA);
        coupon.Value.Should().Be(4);
        var updatedCoupon = coupon with {
            Value = 500,
            OrderId = ExistingOrder2Id
        };

        var result = await couponService.UpdateAsync(CouponIdA, updatedCoupon);
        result.Value.Should().Be(500);
        result.OrderId.Should().Be(ExistingOrder2Id);
    }
    
    [Fact]
    public async Task UpdatePessimistic() {
        var currentTime = AsUtc(new DateTime(2022, 11, 24, 0, 0, 0, DateTimeKind.Local));
        var couponService = CreateCouponService(currentTime);
        var coupon = await couponService.GetByIdAsync(CouponIdA);
        coupon.Value.Should().Be(4);
        var updatedCoupon = coupon with {
            Value = 500,
            OrderId = ExistingOrder2Id
        };

        Func<Task> act = async () => { await couponService.UpdateAsync(CustomerIdA, updatedCoupon); };
        await act.Should().ThrowAsync<CaasItemNotFoundException>();
    }

    [Fact]
    public async Task AddOptimistic() {
        var currentTime = AsUtc(new DateTime(2022, 11, 24, 0, 0, 0, DateTimeKind.Local));
        var couponService = CreateCouponService(currentTime);
        Guid id = Guid.Parse("BA2B00DB-8A9F-4425-80D9-23B2326CFC9A");
        var coupon = new Coupon {
            Id = id,
            ShopId = TestShopId,
            Value = 99
        };

        var result = await couponService.AddAsync(coupon);
        result.Id.Should().Be(id);
        result.Value.Should().Be(99);
    }
    
    [Fact]
    public async Task AddPessimistic() {
        var currentTime = AsUtc(new DateTime(2022, 11, 24, 0, 0, 0, DateTimeKind.Local));
        var couponService = CreateCouponService(currentTime);
        Guid id = Guid.Parse("BA2B00DB-8A9F-4425-80D9-23B2326CFC9A");
        var coupon = new Coupon {
            Id = id,
            ShopId = TestShopId,
            Value = 99,
            OrderId = ExistingOrderId
        };
        
        Func<Task> act = async () => {await couponService.AddAsync(coupon); };
        await act.Should().ThrowAsync<CaasValidationException>();
    }

    [Fact]
    public async Task Delete() {
        var currentTime = AsUtc(new DateTime(2022, 11, 24, 0, 0, 0, DateTimeKind.Local));
        var couponService = CreateCouponService(currentTime);
        var coupon = await couponService.GetByIdAsync(CouponIdE);
        coupon.Id.Should().Be(CouponIdE);

        await couponService.DeleteAsync(CouponIdE);
        
        Func<Task> act = async () => {await couponService.DeleteAsync(CouponIdE); };
        await act.Should().ThrowAsync<CaasItemNotFoundException>();
    }
    
    private ICouponService CreateCouponService(DateTimeOffset currentDate) {
        var customerDao = new MemoryDao<CustomerDataModel>(new List<CustomerDataModel>() {
            new CustomerDataModel { Id = CustomerIdA, ShopId = TestShopId, Name = "Roman Koho", EMail = "test@test.com", CreditCardNumber = "1111222233334444" }
        });
        var customerRepository = new CustomerRepository(customerDao);
        
        var orderItemDao = new MemoryDao<ProductOrderDataModel>(new List<ProductOrderDataModel>() {
            new ProductOrderDataModel() { Id = OrderItemAId, Amount = 2, OrderId = ExistingOrderId, ProductId = ProductAId },
            new ProductOrderDataModel() { Id = OrderItemBId, Amount = 5, OrderId = ExistingOrderId, ProductId = ProductBId }
        });

        var shopDao = new MemoryDao<ShopDataModel>(new List<ShopDataModel>() {
            new ShopDataModel() { Id = TestShopId, Name = TestShopName, AdminId = TestShopAdminId }
        });
        var shopAdminDao = new MemoryDao<ShopAdminDataModel>(new List<ShopAdminDataModel>() {
            new ShopAdminDataModel() { Id = TestShopAdminId, ShopId = TestShopId }
        });
        
        var shopRepository = new ShopRepository(shopDao, shopAdminDao);
        
        var productDao = new MemoryDao<ProductDataModel>(new List<ProductDataModel>() {
            new ProductDataModel() { Id = ProductAId, Name = "ProductA", Price = 10, ShopId = TestShopId },
            new ProductDataModel() { Id = ProductBId, Name = "ProductB", Price = 20, ShopId = TestShopId }
        });
        
        var productRepository = new ProductRepository(productDao, shopRepository);

        var couponDao = new MemoryDao<CouponDataModel>(new List<CouponDataModel>() {
            new CouponDataModel { Id = CouponIdA, ShopId = TestShopId, Value = 4, OrderId = ExistingOrder2Id, CartId = ExistingCart1Id, CustomerId = CustomerIdA},
            new CouponDataModel { Id = CouponIdB, ShopId = TestShopId, Value = 10, OrderId = ExistingOrder2Id, CartId = ExistingCart1Id, CustomerId = CustomerIdA},
            new CouponDataModel { Id = CouponIdC, ShopId = TestShopId, Value = 30, OrderId = ExistingOrderId, CartId = null, CustomerId = CustomerIdB},
            new CouponDataModel { Id = CouponIdD, ShopId = TestShopId, Value = 20, OrderId = ExistingOrderId, CartId = null, CustomerId = CustomerIdB},
            new CouponDataModel { Id = CouponIdE, ShopId = TestShopId, Value = 40, OrderId = null, CartId = null, CustomerId = null},
        });
        
        var couponRepository = new CouponRepository(couponDao);
        
        var cartItemDao = new MemoryDao<ProductCartDataModel>(new List<ProductCartDataModel>() {
            new ProductCartDataModel {ShopId = TestShopId, ProductId = ProductAId, CartId = ExistingCart1Id, Amount = 2 },
            new ProductCartDataModel {ShopId = TestShopId, ProductId = ProductBId, CartId = ExistingCart1Id, Amount = 3 },
            new ProductCartDataModel {ShopId = TestShopId, ProductId = ProductAId, CartId = ExistingCart2Id, Amount = 5 },
            new ProductCartDataModel {ShopId = TestShopId, ProductId = ProductBId, CartId = ExistingCart2Id, Amount = 8 }
        });
        
        var cartDao = new MemoryDao<CartDataModel>(new List<CartDataModel>() {
            new CartDataModel { Id = ExistingCart1Id, ShopId = TestShopId, CustomerId = CustomerIdA, LastAccess = DateTimeOffset.Parse("2022-12-18 16:00:00") },
            new CartDataModel { Id = ExistingCart2Id, ShopId = TestShopId, CustomerId = CustomerIdB, LastAccess = DateTimeOffset.Parse("2022-05-24 12:15:30") }
        });

        var orderDiscountDao = new MemoryDao<OrderDiscountDataModel>(new List<OrderDiscountDataModel>() {
            new OrderDiscountDataModel() { Id = OrderDiscountAId, DiscountName = "Christmas", Discount = 5, OrderId = ExistingOrderId, ShopId = TestShopId },
            new OrderDiscountDataModel() { Id = OrderDiscountBId, DiscountName = "Easter", Discount = 7, OrderId = ExistingOrderId, ShopId = TestShopId },
            new OrderDiscountDataModel() { Id = OrderDiscountCId, DiscountName = "Diwali", Discount = 9, OrderId = ExistingOrderId, ShopId = TestShopId }
        });
        
        var orderItemDiscountDao = new MemoryDao<ProductOrderDiscountDataModel>(new List<ProductOrderDiscountDataModel>() {
            new ProductOrderDiscountDataModel { Id = OrderItemDiscountA, DiscountName = "10% on most expensive product", 
                Discount = (decimal)1.10, ProductOrderId = OrderItemAId, ShopId = TestShopId},
            new ProductOrderDiscountDataModel { Id = OrderItemDiscountB, DiscountName = "5% on every product", 
                Discount = (decimal)0.90, ProductOrderId = OrderItemAId, ShopId = TestShopId},       
        });
        
        var orderDao = new MemoryDao<OrderDataModel>(new List<OrderDataModel>() {
            new OrderDataModel { Id = ExistingOrderId, CreationTime =  DateTimeOffset.Parse("2022-05-01"), ShopId = TestShopId, OrderNumber = 54212, CustomerId = CustomerIdA },
            new OrderDataModel { Id = ExistingOrder2Id, CreationTime = DateTimeOffset.Parse("2021-04-05"), ShopId = TestShopId, OrderNumber = 5, CustomerId = CustomerIdA }
        });

        var orderRepository = new OrderRepository(orderDao, orderItemDao, orderItemDiscountDao, orderDiscountDao, 
            productRepository, customerRepository, couponRepository);
        
        var cartRepository = new CartRepository(cartDao, cartItemDao, productRepository, customerRepository, couponRepository, new StaticSystemClock(currentDate));
        
        return new CouponService(couponRepository, cartRepository, orderRepository, new StaticTenantIdAccessor(TestShopId.ToString()));
    }
    
    private static DateTimeOffset AsUtc(DateTime dateTime) => dateTime.ToUniversalTime().ToDateTimeOffset();
}