using CaaS.Core.CustomerAggregate;

namespace CaaS.Core.OrderAggregate; 

public interface IOrderService {
    Task<Order?> FindOrderById(Guid orderId, CancellationToken cancellationToken = default);

    Task<Order> CreateOrder(Guid customerId, Address billingAddress, CancellationToken cancellationToken = default);
    
    Task<Order> CreateOrderFromCart(Guid cartId, Customer customer, Address billingAddress, CancellationToken cancellationToken = default);
    Task<Order> CreateOrderFromCart(Guid cartId, Address billingAddress, CancellationToken cancellationToken = default);
}