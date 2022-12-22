using CaaS.Core.CustomerAggregate;

namespace CaaS.Core.OrderAggregate; 

public interface IOrderService {
    Task<Order?> FindByIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Order> CreateOrderFromCartAsync(Guid cartId, Guid customerId, Address billingAddress, CancellationToken cancellationToken = default);
    Task<Order> CreateOrderFromCartAsync(Guid cartId, Address billingAddress, CancellationToken cancellationToken = default);
}