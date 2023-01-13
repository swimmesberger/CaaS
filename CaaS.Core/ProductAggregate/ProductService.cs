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
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly ITenantIdAccessor _tenantIdAccessor;

    public ProductService(IProductRepository productRepository, IShopRepository shopRepository, IUnitOfWorkManager unitOfWorkManager,
        ITenantIdAccessor tenantIdAccessor) {
        _productRepository = productRepository;
        _shopRepository = shopRepository;
        _tenantIdAccessor = tenantIdAccessor;
        _unitOfWorkManager = unitOfWorkManager;
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) {
        return await _productRepository.FindByIdAsync(id, cancellationToken);
    }
    
    public async Task<PagedResult<Product>> GetByTextSearchAsync(string? text, PaginationToken? paginationToken = null, CancellationToken cancellationToken = default) {
        return await _productRepository.FindByTextSearchAsync(text, paginationToken, cancellationToken);
    }
    
    public async Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default) {
        await using var uow = _unitOfWorkManager.Begin();
        var shop =  await _shopRepository.FindByIdAsync(_tenantIdAccessor.GetTenantGuid(), cancellationToken);
        if (shop == null) {
            throw new CaasItemNotFoundException();
        }
        product = product with {
            Shop = shop
        };
        
        product = await _productRepository.AddAsync(product, cancellationToken);
        await uow.CompleteAsync(cancellationToken);
        return product;
    }
    
    public async Task<Product> UpdateAsync(Product updatedProduct, CancellationToken cancellationToken = default) {
        if (updatedProduct.Price <= 0) {
            throw new CaasValidationException(new ValidationFailure(nameof(updatedProduct.Price), "Invalid price"));
        }
        await using var uow = _unitOfWorkManager.Begin();
        var oldProduct = await _productRepository.FindByIdAsync(updatedProduct.Id, cancellationToken);
        if (oldProduct == null) {
            throw new CaasItemNotFoundException($"Product '{updatedProduct.Id}' not found");
        }
        
        var shop =  await _shopRepository.FindByIdAsync(_tenantIdAccessor.GetTenantGuid(), cancellationToken);
        if (shop == null) {
            throw new CaasItemNotFoundException();
        }
        
        updatedProduct = updatedProduct with {
            Shop = shop
        };

        updatedProduct = await _productRepository.UpdateAsync(oldProduct, updatedProduct, cancellationToken);
        await uow.CompleteAsync(cancellationToken);
        return updatedProduct;
    }

    public async Task DeleteAsync(Guid productId, CancellationToken cancellationToken = default) {
        await using var uow = _unitOfWorkManager.Begin();
        var product = await _productRepository.FindByIdAsync(productId, cancellationToken);
        if (product == null) {
            throw new CaasItemNotFoundException();
        }
        var updatedProduct = product with {
            Deleted = true
        };

        await _productRepository.UpdateAsync(product, updatedProduct, cancellationToken);
        await uow.CompleteAsync(cancellationToken);
    }
}