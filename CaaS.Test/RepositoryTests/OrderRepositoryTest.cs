using System.Collections.Immutable;
using CaaS.Core.CouponAggregate;
using CaaS.Core.CustomerAggregate;
using CaaS.Core.OrderAggregate;
using CaaS.Core.ProductAggregate;
using CaaS.Core.ShopAggregate;
using CaaS.Infrastructure.CouponData;
using CaaS.Infrastructure.CustomerData;
using CaaS.Infrastructure.OrderData;
using CaaS.Infrastructure.ProductData;
using CaaS.Infrastructure.ShopData;
using CaaS.Test.Common;

namespace CaaS.Test.RepositoryTests; 

public class OrderRepositoryTest  {
    private static readonly Guid TestShopId = new Guid("1AF5037B-16A0-430A-8035-6BCD785CBFB6");
    private const string TestShopName = "TestShop";
    private static readonly Guid ExistingOrderId = new Guid("0EE2F24E-E2DF-4A0E-B055-C804A6672D44");
    private static readonly Guid ExistingOrder2Id = new Guid("AAABBB4E-E2DF-4A0E-B055-C804A6672555");
    private static readonly Guid CustomerIdA = new Guid("99C91EA1-4CA5-9097-DFDB-CF688F0DA31F");
    private static readonly Guid ProductAId = new Guid("05B7F6AB-4409-4417-9F76-34035AC92AE9");
    private static readonly Guid ProductBId = new Guid("DD7E1EA1-6D85-4596-AADB-A4648F7DA31F");
    private static readonly Guid OrderDiscountAId = new Guid("AA1B1EA1-6D85-4596-AADB-CF648F7DA31F");
    private static readonly Guid CouponIdA = new Guid("BB791EA1-3CA5-9097-DFDB-CF648F7DA31F");
    private static readonly Guid OrderItemDiscountA = new Guid("DE123EA1-3CA5-9097-DFDB-CF648F7DA31F");
    
    private ICustomerRepository GetCustomerRepository() {
        var customerDao = new MemoryDao<CustomerDataModel>(new List<CustomerDataModel>() {
            new CustomerDataModel { Id = CustomerIdA, ShopId = TestShopId, Name = "Roman Koho", EMail = "test@test.com", CreditCardNumber = "1111222233334444" }
        });
        return new CustomerRepository(customerDao);
    }
    private IShopRepository GetShopRepository() {

        var testShopAdminId = new Guid("B0077779-E33B-4DFF-938B-47BA4A10854B");
        
        var shopDao = new MemoryDao<ShopDataModel>(new List<ShopDataModel>() {
            new ShopDataModel() { Id = TestShopId, Name = TestShopName, AdminId = testShopAdminId }
        });
        var shopAdminDao = new MemoryDao<ShopAdminDataModel>(new List<ShopAdminDataModel>() {
            new ShopAdminDataModel() { Id = testShopAdminId, ShopId = TestShopId }
        });
        
        var shopRepository = new ShopRepository(shopDao, shopAdminDao);
        return shopRepository;
    }
    private MemoryDao<ProductOrderDataModel> GetOrderItemDao() {
        var orderItemDao = new MemoryDao<ProductOrderDataModel>(new List<ProductOrderDataModel>() {
            new ProductOrderDataModel() { Amount = 2, OrderId = ExistingOrderId, ProductId = ProductAId },
            new ProductOrderDataModel() { Amount = 5, OrderId = ExistingOrderId, ProductId = ProductAId }
        });
        return orderItemDao;
    }
    private IProductRepository GetProductRepository() {
        var productDao = new MemoryDao<ProductDataModel>(new List<ProductDataModel>() {
            new ProductDataModel() { Id = ProductAId, Name = "ProductA", Price = 10, ShopId = TestShopId },
            new ProductDataModel() { Id = ProductBId, Name = "ProductB", Price = 20, ShopId = TestShopId }
        });
        
        var productRepository = new ProductRepository(productDao, GetShopRepository());
        return productRepository;
    }
    private ICouponRepository GetCouponRepository() {
        var couponDao = new MemoryDao<CouponDataModel>(new List<CouponDataModel>() {
            new CouponDataModel { Id = CouponIdA, ShopId = TestShopId, Value = 4, OrderId = ExistingOrderId, CartId = null, RedeemedBy = CustomerIdA}
        });
        var couponRepository = new CouponRepository(couponDao);
        return couponRepository;
    }
    private MemoryDao<OrderDiscountDataModel> GetOrderDiscountDao() {
        var orderDiscountDao = new MemoryDao<OrderDiscountDataModel>(new List<OrderDiscountDataModel>() {
            new OrderDiscountDataModel()
                { Id = OrderDiscountAId, DiscountName = "Christmas", Discount = 5, OrderId = ExistingOrderId, ShopId = TestShopId }
        });
        return orderDiscountDao;
    }
    private MemoryDao<ProductOrderDiscountDataModel> GetProductOrderDiscountDao() {
        var orderItemDiscountDao = new MemoryDao<ProductOrderDiscountDataModel>(new List<ProductOrderDiscountDataModel>() {
            new ProductOrderDiscountDataModel { Id = OrderItemDiscountA, DiscountName = "10% on most expensive product", 
                Discount = (decimal)1.10, ProductOrderId = default, ShopId = TestShopId}
        });
        return orderItemDiscountDao;
    }
    private IOrderRepository GetOrderRepository() {
       
        var orderDao = new MemoryDao<OrderDataModel>(new List<OrderDataModel>() {
            new OrderDataModel { Id = ExistingOrderId, ShopId = TestShopId, OrderNumber = 54212, CustomerId = CustomerIdA, OrderDate = DateTimeOffset.Now },
            new OrderDataModel { Id = ExistingOrder2Id, ShopId = TestShopId, OrderNumber = 5, CustomerId = CustomerIdA, OrderDate = DateTimeOffset.Now }
        });

        var productRepository = GetProductRepository();
        var customerRepository = GetCustomerRepository();
        var couponRepository = GetCouponRepository();
        var orderRepository = new OrderRepository(orderDao, GetOrderItemDao(), GetProductOrderDiscountDao(), GetOrderDiscountDao(), 
                                                    productRepository, customerRepository, couponRepository);
        return orderRepository;
    }
    private async Task<Order> CreateNewOrderDomainModel() {
        var productRepository = GetProductRepository();
        var product = await productRepository.FindByIdAsync(ProductAId);

        var customerRepository = GetCustomerRepository();
        var customer = await customerRepository.FindByIdAsync(CustomerIdA);
        
        Guid orderId = Guid.NewGuid();
        
        var orderItem1 = new OrderItem {
            Id = Guid.NewGuid(),
            Product = product!,
            ShopId = TestShopId,
            OrderId = orderId,
            Amount = 10
        };

        product = await productRepository.FindByIdAsync(ProductAId);
        var orderItem2 = new OrderItem {
            Id = Guid.NewGuid(),
            Product = product!,
            ShopId = TestShopId,
            OrderId = orderId,
            Amount = 10
        };

        var orderItem1Discount1 = new OrderItemDiscount {
            Id = Guid.NewGuid(),
            ShopId = TestShopId,
            OrderItemId = orderItem1.Id,
            DiscountName = "the first OrderItemDiscount of the first product in this order",
            DiscountValue = (decimal)1.99
        };

        var orderItem1Discount2 = new OrderItemDiscount() {
            Id = Guid.NewGuid(),
            ShopId = TestShopId,
            OrderItemId = orderItem1.Id,
            DiscountName = "the second OrderItemDiscount of the first product in this order",
            DiscountValue = (decimal)2.99
        };

        var orderItem2Discount1 = new OrderItemDiscount() {
            Id = Guid.NewGuid(),
            ShopId = TestShopId,
            OrderItemId = orderItem2.Id,
            DiscountName = "the first OrderItemDiscount of the second product in this order",
            DiscountValue = (decimal)0.99
        };

        var orderItem1Discounts = ImmutableArray.Create(orderItem1Discount1, orderItem1Discount2);
        var orderItem2Discounts =  ImmutableArray.Create(orderItem2Discount1);
        orderItem1 = orderItem1 with {
            OrderItemDiscounts = orderItem1Discounts
        };
        orderItem2 = orderItem2 with {
            OrderItemDiscounts = orderItem2Discounts
        };
        
        var orderItems =  ImmutableArray.Create(orderItem1, orderItem2);
        var newOrder = new Order {
            Id = orderId,
            ShopId = TestShopId,
            OrderNumber = 99,
            Items = orderItems,
            Customer = customer!
        };
        
        var orderDiscount = new OrderDiscount {
            Id = Guid.NewGuid(),
            DiscountName = "my test discount",
            DiscountValue = 5,
            ShopId = TestShopId,
            OrderId = newOrder.Id
        };

        var coupon1 = new Coupon {
            Id = Guid.NewGuid(),
            ShopId = TestShopId,
            Value = 5,
            OrderId = orderId,
            CartId = null,
            CustomerId = customer!.Id
        };
        
        var coupon2 = new Coupon {
            Id = Guid.NewGuid(),
            ShopId = TestShopId,
            Value = 10,
            OrderId = orderId,
            CartId = null,
            CustomerId = customer.Id
        };

        var orderDiscounts =  ImmutableArray.Create(orderDiscount);
        var coupons =  ImmutableArray.Create(coupon1, coupon2);
        
        newOrder = newOrder with {
            OrderDiscounts = orderDiscounts,
            Coupons = coupons
        };

        return newOrder;
    }

    [Fact]
    public async Task FindAllOptimistic() {
        var orderRepository = GetOrderRepository();
        var orders = await orderRepository.FindAllAsync();
    }

    [Fact]
    public async Task FindByCustomerIdOptimistic() {
        var orderRepository = GetOrderRepository();
        var orders = await orderRepository.FindByCustomerId(CustomerIdA);
        orders.Count.Should().Be(2);
        orders[0].Id.Should().Be(ExistingOrderId);
        orders[1].Id.Should().Be(ExistingOrder2Id);
    }

    [Fact]
    public async Task AddAddsEntity() {
        var orderToBeAdded = await CreateNewOrderDomainModel();
        var orderRepository = GetOrderRepository();
        var returnedOrder = await orderRepository.AddAsync(orderToBeAdded);
        returnedOrder.Id.Should().Be(orderToBeAdded.Id);
        returnedOrder.Items.Count.Should().Be(2);
        returnedOrder.Items[0].Id.Should().Be(orderToBeAdded.Items[0].Id);
        returnedOrder.Items[1].Id.Should().Be(orderToBeAdded.Items[1].Id);
        returnedOrder.Items[0].OrderItemDiscounts.Count.Should().Be(2);
        returnedOrder.Items[0].OrderItemDiscounts[0].DiscountValue.Should().Be((decimal)1.99);
        returnedOrder.Items[1].OrderItemDiscounts.Count.Should().Be(1);
        returnedOrder.Items[1].OrderItemDiscounts[0].DiscountValue.Should().Be((decimal)0.99);
        returnedOrder.OrderDiscounts.Count.Should().Be(1);
        returnedOrder.OrderDiscounts[0].DiscountName.Should().Be("my test discount");
        returnedOrder.Coupons.Count.Should().Be(2);
        returnedOrder.Coupons[0].Id.Should().Be(orderToBeAdded.Coupons[0].Id);
    }

    [Fact]
    public async Task SingleUpdateOrderNumber() {
        var orderRepository = GetOrderRepository();
        var order = await orderRepository.FindByIdAsync(ExistingOrderId);
        order!.Id.Should().Be(ExistingOrderId);
        order.OrderNumber.Should().Be(54212);

        var orderUpdate = order with {
            OrderNumber = 100
        };

        await orderRepository.UpdateAsync(order, orderUpdate);

        order = await orderRepository.FindByIdAsync(ExistingOrderId);
        order!.Id.Should().Be(ExistingOrderId);
        order.OrderNumber.Should().Be(100);
    }
    
    [Fact]
    public async Task SingleUpdateItemUpdate() {
        var orderRepository = GetOrderRepository();
        var order = await orderRepository.FindByIdAsync(ExistingOrderId);
        var orderItem = order!.Items[0];
        orderItem.Amount.Should().Be(2);
        var orderItemUpdate = orderItem with {
            Amount = 99
        };

        var newOrderItems =  ImmutableArray.Create(orderItemUpdate, order.Items[1]);
        var orderUpdate = order with {
            Items = newOrderItems
        };

        await orderRepository.UpdateAsync(order, orderUpdate);
        order = await orderRepository.FindByIdAsync(ExistingOrderId);
        orderItem = order!.Items[0];
        orderItem.Amount.Should().Be(99);
    }

    [Fact]
    public async Task SingleUpdateAddOrderItem() {
        var orderRepository = GetOrderRepository();
        var order = await orderRepository.FindByIdAsync(ExistingOrderId);
        order!.Items.Count.Should().Be(2);

        var productRepository = GetProductRepository();
        var product = await productRepository.FindByIdAsync(ProductBId);

        var newOrderItem = new OrderItem {
            Id = Guid.NewGuid(),
            Product = product!,
            ShopId = TestShopId,
            OrderId = ExistingOrderId,
            Amount = 55,
            PricePerPiece = 10
        };

        var updatedItems =  ImmutableArray.Create(order.Items[0], order.Items[1], newOrderItem);

        var updatedOrder = order with {
            Items = updatedItems
        };

        await orderRepository.UpdateAsync(order, updatedOrder);
        order = await orderRepository.FindByIdAsync(ExistingOrderId);
        order!.Items.Count.Should().Be(3);
        order.Items[2].Id.Should().Be(newOrderItem.Id);
    }

    [Fact]
    public async Task SingleUpdateRemoveOrderItem() {
        var orderRepository = GetOrderRepository();
        var order = await orderRepository.FindByIdAsync(ExistingOrderId);
        order!.Items.Count.Should().Be(2);
        
        var updatedItems = ImmutableArray.Create(order.Items[0]);
        var updatedOrder = order with {
            Items = updatedItems
        };

        await orderRepository.UpdateAsync(order, updatedOrder);
        order = await orderRepository.FindByIdAsync(ExistingOrderId);
        order!.Items.Count.Should().Be(1);
    }

    [Fact]
    public async Task SingleUpdateOrderUpdateCoupons() {
        var orderRepository = GetOrderRepository();
        var order = await orderRepository.FindByIdAsync(ExistingOrderId);

        order!.Coupons.Count.Should().Be(1);
        order.Coupons[0].Id.Should().Be(CouponIdA);
        order.Coupons[0].Value.Should().Be(4);

        var updatedCoupon = order.Coupons[0] with {
            Value = 10
        };

        var additionalCoupon = new Coupon {
            Id = Guid.NewGuid(),
            ShopId = TestShopId,
            Value = 99,
            OrderId = ExistingOrderId,
            CartId = null,
            CustomerId = CustomerIdA
        };

        var updatedCoupons = ImmutableArray.Create(updatedCoupon, additionalCoupon);
        var updatedOrder = order with {
            Coupons = updatedCoupons
        };

        await orderRepository.UpdateAsync(order, updatedOrder);

        order = await orderRepository.FindByIdAsync(ExistingOrderId);
        order.Should().NotBeNull();
        order!.Coupons.Count.Should().Be(2);
        order.Coupons[0].Value.Should().Be(10);
        order.Coupons[1].Id.Should().Be(additionalCoupon.Id);
        order.Coupons[1].Value.Should().Be(99);
    }
    
}
