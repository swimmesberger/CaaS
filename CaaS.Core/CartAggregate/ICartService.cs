namespace CaaS.Core.CartAggregate; 

public interface ICartService {
    Task<Cart?> GetByIdAsync(Guid cartId, CancellationToken cancellationToken = default);

    Task<Cart> UpdateCartAsync(Cart cart, CancellationToken cancellationToken = default);

    Task<int> DeleteExpiredCartsAsync(CancellationToken cancellationToken = default);
    
    Task DeleteAsync(Guid cartId, CancellationToken cancellationToken = default);

    Task<long> CountAsync(CancellationToken cancellationToken = default);
}