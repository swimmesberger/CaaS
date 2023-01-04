using CaaS.Core.CustomerAggregate;

namespace CaaS.Core.OrderAggregate; 

public interface IOrderService {
    Task<Order?> FindByIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Order> CreateOrderFromCartAsync(Guid cartId, Address billingAddress, Customer? customer = null, CancellationToken cancellationToken = default);
}