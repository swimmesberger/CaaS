namespace CaaS.Core.CartAggregate; 

public interface ICartService {
    Task<Cart?> GetByIdAsync(Guid cartId, CancellationToken cancellationToken = default);

    Task<Cart> AddProductToCartAsync(Guid cartId, Guid productId, int productQuantity, CancellationToken cancellationToken = default);
    
    Task<int> DeleteExpiredCartsAsync(CancellationToken cancellationToken = default);
    
    Task<Cart> RemoveProductFromCartAsync(Guid cartId, Guid productId, CancellationToken cancellationToken = default);
    
    Task<Cart> SetProductQuantityInCartAsync(Guid cartId, Guid productId, int productQuantity, CancellationToken cancellationToken = default);

    Task<Cart> AddCouponToCartAsync(Guid cartId, Guid couponId, CancellationToken cancellationToken = default);
    
    Task DeleteAsync(Guid cartId, CancellationToken cancellationToken = default);

    Task<long> CountAsync(CancellationToken cancellationToken = default);
}