using CaaS.Core.Base;

namespace CaaS.Core.ProductAggregate; 

public interface IProductService {
    Task<CountedResult<Product>> GetByTextSearch(string text, CancellationToken cancellationToken = default);
    Task<Product> AddProduct(Product product, CancellationToken cancellationToken = default);

    Task<Product> SetPriceOfProduct(Guid productId, decimal price, CancellationToken cancellationToken = default);
    Task DeleteProduct(Guid productId, CancellationToken cancellationToken = default);
}