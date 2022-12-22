using CaaS.Core.Base.Exceptions;
using CaaS.Core.Base.Pagination;
using CaaS.Core.ProductAggregate;
using CaaS.Core.ShopAggregate;
using CaaS.Infrastructure.Base.Ado;
using CaaS.Infrastructure.Base.Ado.Query.Parameters;
using CaaS.Infrastructure.Base.Ado.Query.Parameters.Where;
using CaaS.Infrastructure.Base.Repository;

namespace CaaS.Infrastructure.ProductData; 
public class ProductRepository : CrudRepository<ProductDataModel, Product>, IProductRepository {
    public ProductRepository(IDao<ProductDataModel> productDao, IShopRepository shopRepository) : 
            base(productDao, new ProductDomainModelConverter(shopRepository)) {}

    public async Task<PagedResult<Product>> FindByTextSearchAsync(string text, PaginationToken? paginationToken = null, CancellationToken cancellationToken = default) {
        var statementParameters = new StatementParameters() {
            WhereParameters = new WhereParameters(new SearchWhere(new[] {
                new QueryParameter(nameof(ProductDataModel.Name)) { Value = text },
                new QueryParameter(nameof(ProductDataModel.Description)) { Value = text }
            }))
        };
        return await FindByPagedAsync(statementParameters, paginationToken, cancellationToken);
    }
}

internal class ProductDomainModelConverter : IDomainModelConverter<ProductDataModel, Product> {
    public OrderParameters DefaultOrderParameters { get; } = new OrderParameters(nameof(ProductDataModel.Name));
    
    private readonly IShopRepository _shopRepository;

    public ProductDomainModelConverter(IShopRepository shopRepository) {
        _shopRepository = shopRepository;
    }
    
    public ProductDataModel ApplyDomainModel(ProductDataModel dataModel, Product domainModel) {
        return dataModel with {
            Name = domainModel.Name,
            Price = domainModel.Price,
            Description = domainModel.Description,
            DownloadLink = domainModel.DownloadLink,
            Deleted = domainModel.Deleted
        };
    }

    public Product ApplyDataModel(Product domainModel, ProductDataModel dataModel) {
        return domainModel with { ConcurrencyToken = dataModel.GetConcurrencyToken() };
    }

    public ProductDataModel ConvertFromDomain(Product product) {
        return new ProductDataModel() {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Description = product.Description,
            DownloadLink = product.DownloadLink,
            ShopId = product.Shop.Id,
            Deleted = product.Deleted,
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
            Description = dataModel.Description,
            DownloadLink = dataModel.DownloadLink,
            Shop = shop,
            Deleted = dataModel.Deleted,
            ConcurrencyToken = dataModel.GetConcurrencyToken()
        };
    }
}