using CaaS.Core.Base;

namespace CaaS.Core.ProductAggregate; 

public class ProductService : IProductService {
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository) {
        _productRepository = productRepository;
    }
    
    public async Task<CountedResult<Product>> GetByTextSearch(string text, CancellationToken cancellationToken = default) {
        var totalCount = await _productRepository.CountAsync(cancellationToken);
        var items = await _productRepository.FindByTextSearchAsync(text, cancellationToken);
        return new CountedResult<Product>() { TotalCount = totalCount, Items = items };
    }

    public async Task<long> Count(CancellationToken cancellationToken = default) {
        return await _productRepository.CountAsync(cancellationToken);
    }
}