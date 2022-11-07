using CaaS.Core.Entities;

namespace CaaS.Core.Services; 

public interface ICartService {
    Task<Cart?> FindCartById(Guid cartId, CancellationToken cancellationToken = default);
    
    Task<Cart> CreateCart(CancellationToken cancellationToken = default);
    
    Task<Cart> AddProductToCart(Guid cartId, Guid productId, int productQuantity, CancellationToken cancellationToken = default);
    
    Task<Cart> RemoveProductFromCart(Guid cartId, Guid productId, CancellationToken cancellationToken = default);
    
    Task<Cart> SetProductQuantityInCart(Guid cartId, Guid productId, int productQuantity, CancellationToken cancellationToken = default);
}