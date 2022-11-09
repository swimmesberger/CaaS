using CaaS.Core.Entities;

namespace CaaS.Core.Services; 

public interface IOrderService {
    Task<Order?> FindOrderById(Guid orderId, CancellationToken cancellationToken = default);

    Task<Order> CreateOrder(Guid customerId, CancellationToken cancellationToken = default);
    
    Task<Order> CreateOrderFromCart(Guid cartId, CancellationToken cancellationToken = default);

}