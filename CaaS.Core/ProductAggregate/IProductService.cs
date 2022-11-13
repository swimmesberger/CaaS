using CaaS.Core.Base;

namespace CaaS.Core.ProductAggregate; 

public interface IProductService {
    Task<CountedResult<Product>> GetByTextSearch(string text, CancellationToken cancellationToken = default);
}