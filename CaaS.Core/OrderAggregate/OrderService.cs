using System.Collections.Immutable;
using CaaS.Core.Base;
using CaaS.Core.Base.Exceptions;
using CaaS.Core.Base.Tenant;
using CaaS.Core.CartAggregate;
using CaaS.Core.CustomerAggregate;

namespace CaaS.Core.OrderAggregate; 

public class OrderService : IOrderService {
    private readonly IOrderRepository _orderRepository;
    private readonly ITenantIdAccessor _tenantIdAccessor;
    
    private readonly ICustomerRepository _customerRepository;
    private readonly ICartRepository _cartRepository;

    public OrderService(IOrderRepository orderRepository, ITenantIdAccessor tenantIdAccessor, 
        ICustomerRepository customerRepository, ICartRepository cartRepository) {
        _orderRepository = orderRepository;
        _tenantIdAccessor = tenantIdAccessor;
        _customerRepository = customerRepository;
        _cartRepository = cartRepository;
    }

    public async Task<Order?> FindOrderById(Guid orderId, CancellationToken cancellationToken = default) {
        var order = await _orderRepository.FindByIdAsync(orderId, cancellationToken);
        return order ?? null;
    }
    
    public async Task<Order> CreateOrder(Guid customerId ,CancellationToken cancellationToken = default) {
        var shortenedCustomerId = customerId.ToString()[..8];
        var customer = await _customerRepository.FindByIdAsync(customerId, cancellationToken);
        if (customer == null) {
            throw new CaasItemNotFoundException();
        }
        
        var rand = new Random();
        var order = new Order() {
            ShopId = _tenantIdAccessor.GetTenantGuid(),
            Customer = customer,
            OrderDate = DateTimeOffsetProvider.GetNow()
        };
        return await _orderRepository.AddAsync(order, cancellationToken);
    }
    
    public async Task<Order> CreateOrderFromCart(Guid cartId, CancellationToken cancellationToken = default) {
        var cart = await _cartRepository.FindByIdAsync(cartId, cancellationToken);

        if (cart == null) {
            throw new CaasItemNotFoundException();
        }
        
        if (cart.Customer == null) {
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
            Items = orderItems.ToImmutableArray(),
            Coupons = cart.Coupons,
            OrderDiscounts = cart.CartDiscounts,
            OrderDate = DateTimeOffsetProvider.GetNow()
        };

        var savedOrder = await _orderRepository.AddAsync(order);
        return savedOrder;
    }
}