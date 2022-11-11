using CaaS.Core.Exceptions;
using CaaS.Core.Product;
using CaaS.Core.Shop;
using CaaS.Infrastructure.Base.Ado;
using CaaS.Infrastructure.Base.Ado.Model;
using CaaS.Infrastructure.Base.Repository;
using CaaS.Infrastructure.Product.DataModel;

namespace CaaS.Infrastructure.Product; 
public class ProductRepository : CrudRepository<ProductDataModel, Core.Product.Entities.Product>, IProductRepository {
    public ProductRepository(IDao<ProductDataModel> productDao, IShopRepository shopRepository) : 
            base(productDao, new ProductDomainModelConvert(shopRepository)) {}
}

internal class ProductDomainModelConvert : IDomainModelConverter<ProductDataModel, Core.Product.Entities.Product> {
    public IEnumerable<OrderParameter> DefaultOrderParameters { get; } = OrderParameter.From(nameof(ProductDataModel.Name));
    
    private readonly IShopRepository _shopRepository;

    public ProductDomainModelConvert(IShopRepository shopRepository) {
        _shopRepository = shopRepository;
    }
    
    public ProductDataModel ApplyDomainModel(ProductDataModel dataModel, Core.Product.Entities.Product domainModel) {
        return dataModel with {
            Name = domainModel.Name,
            Price = domainModel.Price,
        };
    }

    public ProductDataModel ConvertFromDomain(Core.Product.Entities.Product product) {
        return new ProductDataModel() {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            ShopId = product.Shop.Id,
            RowVersion = product.GetRowVersion()
        };
    }
    
    public async ValueTask<Core.Product.Entities.Product> ConvertToDomain(ProductDataModel dataModel, CancellationToken cancellationToken) {
        return (await ConvertToDomain(new List<ProductDataModel>() { dataModel }, cancellationToken)).First();
    }

    public async Task<IReadOnlyList<Core.Product.Entities.Product>> ConvertToDomain(IAsyncEnumerable<ProductDataModel> dataModels, 
            CancellationToken cancellationToken = default) {
        var products = await dataModels.ToListAsync(cancellationToken);
        return await ConvertToDomain(products, cancellationToken);
    }

    public async Task<IReadOnlyList<Core.Product.Entities.Product>> ConvertToDomain(IReadOnlyCollection<ProductDataModel> products, 
            CancellationToken cancellationToken = default) {
        var shopIds = products.Select(p => p.ShopId).ToHashSet();
        var shopDict = (await _shopRepository.FindByIdsAsync(shopIds, cancellationToken))
                .ToDictionary(s => s.Id, s => s);
        return products.Select(dataModel => ConvertToDomain(dataModel, shopDict)).ToList();
    }
    
    public Core.Product.Entities.Product ConvertToDomain(ProductDataModel dataModel, IReadOnlyDictionary<Guid, Core.Shop.Entities.Shop> shopDict) {
        if (!shopDict.TryGetValue(dataModel.ShopId, out var shop)) {
            throw new CaasDomainMappingException($"Failed to find shop {dataModel.ShopId} for product {dataModel.Id}");
        }
        return new Core.Product.Entities.Product() {
            Id = dataModel.Id,
            Name = dataModel.Name,
            Price = dataModel.Price,
            Shop = shop,
            ConcurrencyToken = dataModel.GetConcurrencyToken()
        };
    }
}