using CaaS.Core.Base.Pagination;

namespace CaaS.Core.ProductAggregate; 

public interface IProductService {
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<Product>> GetByTextSearchAsync(string text, PaginationToken? paginationToken = null, CancellationToken cancellationToken = default);
    Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default);
    Task<Product> UpdateAsync(Guid productId, Product updatedProduct, CancellationToken cancellationToken = default);
    Task<Product> SetPriceAsync(Guid productId, decimal price, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid productId, CancellationToken cancellationToken = default);
}