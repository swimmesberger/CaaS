using System.Collections.Immutable;
using CaaS.Core.Entities;
using CaaS.Core.Exceptions;
using CaaS.Core.Repositories;
using CaaS.Infrastructure.Ado.Base;
using CaaS.Infrastructure.Ado.Model;
using CaaS.Infrastructure.DataModel;
using CaaS.Infrastructure.Repositories.Base;

namespace CaaS.Infrastructure.Repositories;

internal class CartItemRepository {
    private IDao<ProductCartDataModel> Dao { get; }
    private CartItemDomainModelConvert Converter { get; }

    public CartItemRepository(IDao<ProductCartDataModel> dao, IProductRepository productRepository) {
        Dao = dao;
        Converter = new CartItemDomainModelConvert(productRepository);
    }

    public async Task<CartItem?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) {
        var dataModel = await Dao.FindByIdAsync(id, cancellationToken);
        if (dataModel == null) return null;
        return await Converter.ConvertToDomain(dataModel, cancellationToken);
    }
    
    public async Task<List<CartItem>> FindByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default) {
        var dataModel = Dao.FindByIdsAsync(ids, cancellationToken);
        return await Converter.ConvertToDomain(dataModel, cancellationToken);
    }

    public async Task<Dictionary<Guid, IReadOnlyList<CartItem>>> FindByCartIds(IEnumerable<Guid> cartIds, CancellationToken cancellationToken = default) {
        return (await Converter
                        .ConvertToDomain(Dao
                        .FindBy(StatementParameters.CreateWhere(nameof(ProductCartDataModel.CartId), cartIds), cancellationToken), cancellationToken))
                .GroupBy(i => i.CartId)
                .ToDictionary(grp => grp.Key, grp => (IReadOnlyList<CartItem>)grp.ToImmutableList());
    }

    public async Task AddAsync(IEnumerable<CartItem> entities, CancellationToken cancellationToken = default) {
        var dataModels = Converter.ConvertFromDomain(entities);
        await Dao.AddAsync(dataModels, cancellationToken);
    }

    public async Task UpdateAsync(IEnumerable<CartItem> entities, CancellationToken cancellationToken = default) {
        var dataModels = Converter.ConvertFromDomain(entities);
        await Dao.UpdateAsync(dataModels, cancellationToken);
    }

    private class CartItemDomainModelConvert : IDomainReadModelConverter<ProductCartDataModel, CartItem> {
        // CreationTime = time added to cart
        public IEnumerable<OrderParameter>? DefaultOrderParameters { get; } = OrderParameter.From(nameof(ProductCartDataModel.CreationTime));
        private readonly IProductRepository _productRepository;

        public CartItemDomainModelConvert(IProductRepository productRepository) {
            _productRepository = productRepository;
        }

        public async ValueTask<CartItem> ConvertToDomain(ProductCartDataModel dataModel, CancellationToken cancellationToken) {
            return (await ConvertToDomain(new List<ProductCartDataModel>() { dataModel }, cancellationToken)).First();
        }
        
        public async Task<List<CartItem>> ConvertToDomain(IAsyncEnumerable<ProductCartDataModel> dataModels, CancellationToken cancellationToken = default) {
            var items = await dataModels.ToListAsync(cancellationToken);
            return await ConvertToDomain(items, cancellationToken);
        }

        public async Task<List<CartItem>> ConvertToDomain(IReadOnlyCollection<ProductCartDataModel> dataModels, CancellationToken cancellationToken = default) {
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

        public List<ProductCartDataModel> ConvertFromDomain(IEnumerable<CartItem> domainModels)
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