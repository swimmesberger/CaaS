namespace CaaS.Core.CartAggregate; 

public interface ICartService {
    Task<Cart?> GetCartById(Guid cartId, CancellationToken cancellationToken = default);

    Task<Cart> AddProductToCart(Guid cartId, Guid productId, int productQuantity, CancellationToken cancellationToken = default);
    Task<int> DeleteExpiredCarts(CancellationToken cancellationToken = default);
    
    Task<Cart> RemoveProductFromCart(Guid cartId, Guid productId, CancellationToken cancellationToken = default);
    
    Task<Cart> SetProductQuantityInCart(Guid cartId, Guid productId, int productQuantity, CancellationToken cancellationToken = default);
    Task DeleteCart(Guid cartId, CancellationToken cancellationToken = default);
}