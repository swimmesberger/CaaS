using CaaS.Core.Base;
using CaaS.Core.Base.Exceptions;
using CaaS.Core.Base.Tenant;
using CaaS.Core.ShopAggregate;
using FluentValidation.Results;

namespace CaaS.Core.ProductAggregate; 

public class ProductService : IProductService {
    private readonly IProductRepository _productRepository;
    private readonly IShopRepository _shopRepository;
    private readonly ITenantIdAccessor _tenantIdAccessor;

    public ProductService(IProductRepository productRepository, IShopRepository shopRepository, ITenantIdAccessor tenantIdAccessor) {
        _productRepository = productRepository;
        _shopRepository = shopRepository;
        _tenantIdAccessor = tenantIdAccessor;
    }
    
    public async Task<CountedResult<Product>> GetByTextSearch(string text, CancellationToken cancellationToken = default) {
        var totalCount = await _productRepository.CountAsync(cancellationToken);
        var items = await _productRepository.FindByTextSearchAsync(text, cancellationToken);
        return new CountedResult<Product>() { TotalCount = totalCount, Items = items };
    }
    public async Task<Product> AddProduct(Product product, CancellationToken cancellationToken = default) {
        var shop =  await _shopRepository.FindByIdAsync(_tenantIdAccessor.GetTenantGuid(), cancellationToken);
        if (shop == null) {
            throw new CaasItemNotFoundException();
        }
        product = product with{ Shop = shop };

        return await _productRepository.AddAsync(product, cancellationToken);
    }
    public async Task<Product> SetPriceOfProduct(Guid productId, decimal price, CancellationToken cancellationToken = default) {
        if (price <= 0) {
            throw new CaasValidationException(new ValidationFailure(nameof(price), "Invalid price"));
        }
        var product = await _productRepository.FindByIdAsync(productId, cancellationToken);
        if (product == null) {
            throw new CaasItemNotFoundException();
        }
        product = product with {
            Price = price
        };
        return await _productRepository.UpdateAsync(product, cancellationToken);
    }
    public async Task DeleteProduct(Guid productId, CancellationToken cancellationToken = default) {
        var product = await _productRepository.FindByIdAsync(productId, cancellationToken);
        if (product == null) {
            throw new CaasItemNotFoundException();
        }
        product = product with {
            Deleted = true
        };
        
        await _productRepository.UpdateAsync(product, cancellationToken);
    }

    public async Task<long> Count(CancellationToken cancellationToken = default) {
        return await _productRepository.CountAsync(cancellationToken);
    }
}