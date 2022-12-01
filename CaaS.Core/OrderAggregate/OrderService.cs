using System.Collections.Immutable;
using CaaS.Core.Base;
using CaaS.Core.Base.Exceptions;
using CaaS.Core.Base.Tenant;
using CaaS.Core.CartAggregate;
using CaaS.Core.CustomerAggregate;

namespace CaaS.Core.OrderAggregate; 

public class OrderService : IOrderService {
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ICartRepository _cartRepository;
    private readonly ITenantIdAccessor _tenantIdAccessor;

    public OrderService(IOrderRepository orderRepository, ICustomerRepository customerRepository, 
        ICartRepository cartRepository, ITenantIdAccessor tenantIdAccessor) {
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
        _cartRepository = cartRepository;
        _tenantIdAccessor = tenantIdAccessor;
    }

    public async Task<Order?> FindOrderById(Guid orderId, CancellationToken cancellationToken = default) {
        var order = await _orderRepository.FindByIdAsync(orderId, cancellationToken);
        return order ?? null;
    }
    
    public async Task<Order> CreateOrder(Guid customerId ,CancellationToken cancellationToken = default) {
        var customer = await _customerRepository.FindByIdAsync(customerId, cancellationToken);
        if (customer == null) {
            throw new CaasItemNotFoundException();
        }
        
        var order = new Order() {
            ShopId = _tenantIdAccessor.GetTenantGuid(),
            Customer = customer,
            OrderDate = DateTimeOffsetProvider.GetNow()
        };
        return await _orderRepository.AddAsync(order, cancellationToken);
    }
    
    public async Task<Order> CreateOrderFromCart(Guid cartId, Address billingAddress, CancellationToken cancellationToken = default) {
        var cart = await _cartRepository.FindByIdAsync(cartId, cancellationToken);

        if (cart == null) {
            throw new CaasItemNotFoundException();
        }
        
        if (cart.Customer == null || cart.Customer.Id.Equals(Guid.Empty)) {
            throw new CaasItemNotFoundException("a cart needs a customer when an order is created out of it");
        }
        
        var orderId = Guid.NewGuid();
        var orderItems = new List<OrderItem>();
        foreach (var item in cart.Items) {
            var orderItem = new OrderItem {
                Id = item.Id,
                Product = item.Product,
                ShopId = item.ShopId,
                OrderId = orderId,
                Amount = item.Amount,
                OrderItemDiscounts = item.CartItemDiscounts,
                PricePerPiece = item.Product.Price
            };
            orderItems.Add(orderItem);
        }

        var order = new Order {
            Id = orderId,
            ShopId = _tenantIdAccessor.GetTenantGuid(),
            Customer = cart.Customer,
            Items = orderItems.ToImmutableList(),
            Coupons = cart.Coupons,
            Address = billingAddress,
            OrderDiscounts = cart.CartDiscounts,
            OrderDate = DateTimeOffsetProvider.GetNow()
        };

        return await _orderRepository.AddAsync(order, cancellationToken);
    }
}