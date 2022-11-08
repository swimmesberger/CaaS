using CaaS.Core.Entities;
using CaaS.Core.Exceptions;
using CaaS.Core.Repositories;
using CaaS.Infrastructure.Ado.Base;
using CaaS.Infrastructure.Ado.Model;
using CaaS.Infrastructure.DataModel;
using CaaS.Infrastructure.Repositories.Base;

namespace CaaS.Infrastructure.Repositories; 
public class ProductRepository : CrudRepository<ProductDataModel, Product>, IProductRepository {
    public ProductRepository(IDao<ProductDataModel> productDao, IShopRepository shopRepository) : 
            base(productDao, new ProductDomainModelConvert(shopRepository)) {}
}

internal class ProductDomainModelConvert : IDomainModelConverter<ProductDataModel, Product> {
    public IEnumerable<OrderParameter> DefaultOrderParameters { get; } = OrderParameter.From(nameof(ProductDataModel.Name));
    
    private readonly IShopRepository _shopRepository;

    public ProductDomainModelConvert(IShopRepository shopRepository) {
        _shopRepository = shopRepository;
    }
    
    public ProductDataModel ApplyDomainModel(ProductDataModel dataModel, Product domainModel) {
        return dataModel with {
            Name = domainModel.Name,
            Price = domainModel.Price,
        };
    }

    public ProductDataModel ConvertFromDomain(Product product) {
        return new ProductDataModel() {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            ShopId = product.Shop.Id,
            RowVersion = product.GetRowVersion()
        };
    }
    
    public async ValueTask<Product> ConvertToDomain(ProductDataModel dataModel, CancellationToken cancellationToken) {
        return (await ConvertToDomain(new List<ProductDataModel>() { dataModel }, cancellationToken)).First();
    }

    public async Task<IReadOnlyList<Product>> ConvertToDomain(IAsyncEnumerable<ProductDataModel> dataModels, 
            CancellationToken cancellationToken = default) {
        var products = await dataModels.ToListAsync(cancellationToken);
        return await ConvertToDomain(products, cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> ConvertToDomain(IReadOnlyCollection<ProductDataModel> products, 
            CancellationToken cancellationToken = default) {
        var shopIds = products.Select(p => p.ShopId).ToHashSet();
        var shopDict = (await _shopRepository.FindByIdsAsync(shopIds, cancellationToken))
                .ToDictionary(s => s.Id, s => s);
        return products.Select(dataModel => ConvertToDomain(dataModel, shopDict)).ToList();
    }
    
    public Product ConvertToDomain(ProductDataModel dataModel, IReadOnlyDictionary<Guid, Shop> shopDict) {
        if (!shopDict.TryGetValue(dataModel.ShopId, out var shop)) {
            throw new CaasDomainMappingException($"Failed to find shop {dataModel.ShopId} for product {dataModel.Id}");
        }
        return new Product() {
            Id = dataModel.Id,
            Name = dataModel.Name,
            Price = dataModel.Price,
            Shop = shop,
            ConcurrencyToken = dataModel.GetConcurrencyToken()
        };
    }
}