using CaaS.Core.Order;
using CaaS.Infrastructure.Coupon;
using CaaS.Infrastructure.Customer;
using CaaS.Infrastructure.Order;
using CaaS.Infrastructure.Order.DataModel;
using CaaS.Infrastructure.Product;
using CaaS.Infrastructure.Product.DataModel;
using CaaS.Infrastructure.Shop;
using CaaS.Infrastructure.Shop.DataModel;
using CaaS.Test.Common;

namespace CaaS.Test; 

public class OrderServiceTest {
    private static readonly Guid TestShopId = new Guid("1AF5037B-16A0-430A-8035-6BCD785CBFB6");
    private const string TestShopName = "TestShop";
    private static readonly Guid ExistingOrderId = new Guid("0EE2F24E-E2DF-4A0E-B055-C804A6672D44");
    private static readonly Guid CustomerIdA = new Guid("99C91EA1-4CA5-9097-DFDB-CF688F0DA31F");
    private static readonly Guid ProductAId = new Guid("05B7F6AB-4409-4417-9F76-34035AC92AE9");
    private static readonly Guid ProductBId = new Guid("DD7E1EA1-6D85-4596-AADB-A4648F7DA31F");
    private static readonly Guid OrderDiscountAId = new Guid("AA1B1EA1-6D85-4596-AADB-CF648F7DA31F");
    private static readonly Guid CouponIdA = new Guid("BB791EA1-3CA5-9097-DFDB-CF648F7DA31F");
    private static readonly Guid OrderItemDiscountA = new Guid("DE123EA1-3CA5-9097-DFDB-CF648F7DA31F");

    [Fact]
    public async Task CreateOrderOptimistic() {
        var orderService = CreateOrderService();
        
        var createOrder = await orderService.CreateOrder(CustomerIdA);
        createOrder.Should().NotBeNull();
        createOrder.ShopId.Should().Be(TestShopId);
        createOrder.Items.Should().BeEmpty();
    }
    
    [Fact]
    public async Task FindOrderByIdOptimistic() {
        var orderService = CreateOrderService();

        var foundOrder = await orderService.FindOrderById(ExistingOrderId);
        foundOrder.Should().NotBeNull();
        foundOrder!.Id.Should().Be(ExistingOrderId);
        foundOrder.Items.Count.Should().Be(2);
    }

    [Fact]
    public Task AddProductToOrderWhenNoExisting() {
        // TODO: implement
        return Task.CompletedTask;
    }

    private IOrderService CreateOrderService() {
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
            new CustomerDataModel { Id = CustomerIdA, ShopId = TestShopId, Name = "Roman Koho", EMail = "test@test.com", CreditCardNumber = "1111222233334444" }
        });
        
        var orderDao = new MemoryDao<OrderDataModel>(new List<OrderDataModel>() {
            new OrderDataModel { Id = ExistingOrderId, ShopId = TestShopId, OrderNumber = 54212, CustomerId = CustomerIdA, OrderDate = DateTimeOffset.Now }
        });
        
        var orderItemDao = new MemoryDao<ProductOrderDataModel>(new List<ProductOrderDataModel>() {
            new ProductOrderDataModel() { Amount = 2, OrderId = ExistingOrderId, ProductId = ProductAId },
            new ProductOrderDataModel() { Amount = 5, OrderId = ExistingOrderId, ProductId = ProductAId }
        });

        var orderDiscountDao = new MemoryDao<OrderDiscountDataModel>(new List<OrderDiscountDataModel>() {
            new OrderDiscountDataModel() { Id = OrderDiscountAId, DiscountName = "Christmas", Discount = 5, OrderId = ExistingOrderId, ShopId = TestShopId}
        });

        var couponDao = new MemoryDao<CouponDataModel>(new List<CouponDataModel>() {
            new CouponDataModel { Id = CouponIdA, ShopId = TestShopId, Value = 4, OrderId = ExistingOrderId, CartId = null, RedeemedBy = CustomerIdA}
        });

        var orderItemDiscountDao = new MemoryDao<ProductOrderDiscountDataModel>(new List<ProductOrderDiscountDataModel>() {
            new ProductOrderDiscountDataModel { Id = OrderItemDiscountA, DiscountName = "10% on most expensive product", 
                Discount = (decimal)1.10, ProductOrderId = default, ShopId = TestShopId}
        });
        
        var shopRepository = new ShopRepository(shopDao, shopAdminDao);
        var productRepository = new ProductRepository(productDao, shopRepository);
        var customerRepository = new CustomerRepository(customerDao);
        var couponRepository = new CouponRepository(couponDao);

        var tenantIdAccessor = new StaticTenantIdAccessor(TestShopId.ToString());
        var orderRepository = new OrderRepository(orderDao, orderItemDao, orderItemDiscountDao, orderDiscountDao, 
                                                    productRepository, customerRepository, couponRepository);
        
        return new OrderService(orderRepository, tenantIdAccessor, customerRepository, productRepository);
    }
}