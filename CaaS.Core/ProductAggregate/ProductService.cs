using CaaS.Core.Base.Exceptions;
using System.ComponentModel.DataAnnotations;
using CaaS.Core.Base;
using CaaS.Core.Base.Exceptions;
using CaaS.Core.Base.Pagination;
using CaaS.Core.Base.Tenant;
using CaaS.Core.Base.Validation;
using CaaS.Core.ShopAggregate;

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

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) {
        return await _productRepository.FindByIdAsync(id, cancellationToken);
    }
    
    public async Task<PagedResult<Product>> GetByTextSearchAsync(string text, PaginationToken? paginationToken = null, CancellationToken cancellationToken = default) {
        return await _productRepository.FindByTextSearchAsync(text, paginationToken, cancellationToken);
    }
    
    public async Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default) {
        var shop =  await _shopRepository.FindByIdAsync(_tenantIdAccessor.GetTenantGuid(), cancellationToken);
        if (shop == null) {
            throw new CaasItemNotFoundException();
        }
        product = product with{ Shop = shop };

        return await _productRepository.AddAsync(product, cancellationToken);
    }
    public async Task<Product> UpdateAsync(Guid productId, Product updatedProduct, CancellationToken cancellationToken = default) {
        var oldProduct = await _productRepository.FindByIdAsync(productId, cancellationToken);
        if (oldProduct == null) {
            throw new CaasItemNotFoundException($"Product '{productId}' not found");
        }
        
        var shop =  await _shopRepository.FindByIdAsync(_tenantIdAccessor.GetTenantGuid(), cancellationToken);
        if (shop == null) {
            throw new CaasItemNotFoundException();
        }
        
        updatedProduct = updatedProduct with {
            Id = productId,
            Shop = shop
        };

        return await _productRepository.UpdateAsync(oldProduct, updatedProduct, cancellationToken);
    }
    public async Task<Product> SetPriceAsync(Guid productId, decimal price, CancellationToken cancellationToken = default) {
        if (price <= 0) {
            throw new CaasValidationException(new ValidationFailure(nameof(price), "Invalid price"));
        }
        var product = await _productRepository.FindByIdAsync(productId, cancellationToken);
        if (product == null) {
            throw new CaasItemNotFoundException();
        }
        
        var updatedProduct = product with {
            Price = price
        };
        return await _productRepository.UpdateAsync(product, updatedProduct, cancellationToken);
    }
    public async Task DeleteAsync(Guid productId, CancellationToken cancellationToken = default) {
        var product = await _productRepository.FindByIdAsync(productId, cancellationToken);
        if (product == null) {
            throw new CaasItemNotFoundException();
        }
        var updatedProduct = product with {
            Deleted = true
        };
        
        await _productRepository.UpdateAsync(product, updatedProduct, cancellationToken);
    }

    public async Task<long> Count(CancellationToken cancellationToken = default) {
        return await _productRepository.CountAsync(cancellationToken);
    }
}