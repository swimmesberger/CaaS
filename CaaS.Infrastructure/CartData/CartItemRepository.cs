using System.Collections.Immutable;
using CaaS.Core.Base;
using CaaS.Core.Base.Exceptions;
using CaaS.Core.CartAggregate;
using CaaS.Core.ProductAggregate;
using CaaS.Infrastructure.Base.Ado;
using CaaS.Infrastructure.Base.Ado.Query.Parameters;
using CaaS.Infrastructure.Base.Repository;

namespace CaaS.Infrastructure.CartData;

internal class CartItemRepository : IRepository {
    private IDao<ProductCartDataModel> Dao { get; }
    private CartItemDomainModelConvert Converter { get; }

    public CartItemRepository(IDao<ProductCartDataModel> dao, IProductRepository productRepository) {
        Dao = dao;
        Converter = new CartItemDomainModelConvert(productRepository);
    }
    
    public async Task<IReadOnlyDictionary<Guid, IReadOnlyList<CartItem>>> FindByCartIds(IReadOnlyCollection<Guid> cartIds, CancellationToken cancellationToken = default) {
        if (cartIds.Count.Equals(0)) {
            return new Dictionary<Guid, IReadOnlyList<CartItem>>();
        }
        
        return (await Converter
                        .ConvertToDomain(Dao
                        .FindBy(StatementParameters.CreateWhere(nameof(ProductCartDataModel.CartId), cartIds), cancellationToken), cancellationToken))
                .GroupBy(i => i.CartId)
                .ToDictionary(grp => grp.Key, grp => (IReadOnlyList<CartItem>)grp.ToImmutableArray());
    }

    public async Task AddAsync(IEnumerable<CartItem> entities, CancellationToken cancellationToken = default) {
        var dataModels = Converter.ConvertFromDomain(entities);
        await Dao.AddAsync(dataModels, cancellationToken);
    }
    
    public async Task UpdateProductsAsync(IEnumerable<CartItem> oldDomainModels, IEnumerable<CartItem> newDomainModels,
        CancellationToken cancellationToken = default) {
        var oldDataModels = oldDomainModels.Select(Converter.ConvertFromDomain);
        var newDataModels = newDomainModels.Select(Converter.ConvertFromDomain);
        await Dao.ApplyAsync(oldDataModels, newDataModels.ToList(), cancellationToken);
    }

    private class CartItemDomainModelConvert : IDomainReadModelConverter<ProductCartDataModel, CartItem> {
        // CreationTime = time added to cart
        public OrderParameters DefaultOrderParameters { get; } = new OrderParameters(nameof(ProductCartDataModel.CreationTime));
        private readonly IProductRepository _productRepository;

        public CartItemDomainModelConvert(IProductRepository productRepository) {
            _productRepository = productRepository;
        }

        public async ValueTask<CartItem> ConvertToDomain(ProductCartDataModel dataModel, CancellationToken cancellationToken) {
            return (await ConvertToDomain(ImmutableList.Create(dataModel), cancellationToken)).First();
        }
        
        public async Task<IReadOnlyList<CartItem>> ConvertToDomain(IAsyncEnumerable<ProductCartDataModel> dataModels, CancellationToken cancellationToken = default) {
            var items = await dataModels.ToListAsync(cancellationToken);
            return await ConvertToDomain(items, cancellationToken);
        }

        public async Task<IReadOnlyList<CartItem>> ConvertToDomain(IReadOnlyCollection<ProductCartDataModel> dataModels, CancellationToken cancellationToken = default) {
            var productIds = dataModels.Select(p => p.ProductId).ToHashSet();
            var productDict = (await _productRepository
                            .FindByIdsAsync(productIds, cancellationToken))
                    .ToDictionary(s => s.Id, s => s);
            var domainModels = new List<CartItem>();
            foreach (var dataModel in dataModels) {
                if (!productDict.TryGetValue(dataModel.ProductId, out var product)) {
                    throw new CaasDomainMappingException($"Failed to find product {dataModel.ProductId} for cart-item {dataModel.Id}");
                }
                domainModels.Add(new CartItem() {
                    Id = dataModel.Id,
                    CartId = dataModel.CartId,
                    ShopId = dataModel.ShopId,
                    Product = product,
                    Amount = dataModel.Amount,
                    ConcurrencyToken = dataModel.GetConcurrencyToken()
                });
            }
            return domainModels;
        }

        public IReadOnlyList<ProductCartDataModel> ConvertFromDomain(IEnumerable<CartItem> domainModels)
            => domainModels.Select(ConvertFromDomain).ToList();
        
        public ProductCartDataModel ConvertFromDomain(CartItem domainModel) {
            return new ProductCartDataModel() {
                Id = domainModel.Id,
                Amount = domainModel.Amount,
                CartId = domainModel.CartId,
                ProductId = domainModel.Product.Id,
                ShopId = domainModel.ShopId,
                RowVersion = domainModel.GetRowVersion()
            };
        }
    }
}