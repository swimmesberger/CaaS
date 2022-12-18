using CaaS.Core.Base;
using CaaS.Core.Base.Pagination;

namespace CaaS.Core.ProductAggregate; 

public interface IProductService {
    Task<Product?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<Product>> GetByTextSearch(string text, PaginationToken? paginationToken = null, CancellationToken cancellationToken = default);
    Task<Product> AddProduct(Product product, CancellationToken cancellationToken = default);
    Task<Product> UpdateProduct(Guid productId, Product updatedProduct, CancellationToken cancellationToken = default);

    Task<Product> SetPriceOfProduct(Guid productId, decimal price, CancellationToken cancellationToken = default);
    Task DeleteProduct(Guid productId, CancellationToken cancellationToken = default);
}