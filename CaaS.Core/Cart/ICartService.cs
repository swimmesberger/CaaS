namespace CaaS.Core.Cart; 

public interface ICartService {
    Task<Entities.Cart?> FindCartById(Guid cartId, CancellationToken cancellationToken = default);
    
    Task<Entities.Cart> CreateCart(CancellationToken cancellationToken = default);
    
    Task<Entities.Cart> AddProductToCart(Guid cartId, Guid productId, int productQuantity, CancellationToken cancellationToken = default);
    
    Task<Entities.Cart> RemoveProductFromCart(Guid cartId, Guid productId, CancellationToken cancellationToken = default);
    
    Task<Entities.Cart> SetProductQuantityInCart(Guid cartId, Guid productId, int productQuantity, CancellationToken cancellationToken = default);
}