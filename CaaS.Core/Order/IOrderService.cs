namespace CaaS.Core.Order; 

public interface IOrderService {
    Task<Entities.Order?> FindOrderById(Guid orderId, CancellationToken cancellationToken = default);

    Task<Entities.Order> CreateOrder(Guid customerId, CancellationToken cancellationToken = default);
    
    Task<Entities.Order> CreateOrderFromCart(Guid cartId, CancellationToken cancellationToken = default);

}