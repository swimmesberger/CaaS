using System.Collections.Immutable;
using CaaS.Core.Discount;
using CaaS.Core.Entities;
using CaaS.Core.Exceptions;
using CaaS.Core.Repositories;
using CaaS.Core.Repositories.Base;
using CaaS.Infrastructure.Ado.Base;
using CaaS.Infrastructure.Ado.Model;
using CaaS.Infrastructure.DataModel;
using CaaS.Infrastructure.Repositories.Base;

namespace CaaS.Infrastructure.Repositories;

internal class OrderItemRepository {
    private IDao<ProductOrderDataModel> Dao { get; }
    private OrderItemDomainModelConvert Converter { get; }

    public OrderItemRepository(IDao<ProductOrderDataModel> dao,
        IProductRepository productRepository, IDao<ProductOrderDiscountDataModel> orderItemDiscountDao) {
        Dao = dao;
        Converter = new OrderItemDomainModelConvert(productRepository, orderItemDiscountDao);
    }

    public async Task<OrderItem?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) {
        var dataModel = await Dao.FindByIdAsync(id, cancellationToken);
        if (dataModel == null) return null;
        return await Converter.ConvertToDomain(dataModel, cancellationToken);
    }

    public async Task<IReadOnlyList<OrderItem>> FindByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default) {
        var dataModel = Dao.FindByIdsAsync(ids, cancellationToken);
        return await Converter.ConvertToDomain(dataModel, cancellationToken);
    }

    public async Task<IReadOnlyList<OrderItem>> FindByOrderId(Guid orderId, CancellationToken cancellationToken = default) {
        return (await Converter
                .ConvertToDomain(Dao
                    .FindBy(StatementParameters.CreateWhere(nameof(ProductOrderDataModel.OrderId), orderId), cancellationToken), cancellationToken))
            .ToImmutableList();
    }
    
    public async Task<Dictionary<Guid, IReadOnlyList<OrderItem>>> FindByOrderIds(IEnumerable<Guid> orderIds,
        CancellationToken cancellationToken = default) {
        return (await Converter
                .ConvertToDomain(Dao
                    .FindBy(StatementParameters.CreateWhere(nameof(ProductOrderDataModel.OrderId), orderIds), cancellationToken), cancellationToken))
            .GroupBy(i => i.OrderId)
            .ToDictionary(grp => grp.Key, grp => (IReadOnlyList<OrderItem>)grp.ToImmutableList());
    }
    
    public async Task AddAsync(OrderItem entity, CancellationToken cancellationToken = default) {
        await Converter.OrderItemDiscountRepository.AddAsync(entity.OrderItemDiscounts, cancellationToken);
        var dataModel = Converter.ConvertFromDomain(entity);
        await Dao.AddAsync(dataModel, cancellationToken);
    }
    
    public async Task AddAsync(IEnumerable<OrderItem> entities, CancellationToken cancellationToken = default) {
        var domainModels = entities.ToList();
        var orderItemDiscounts = domainModels.SelectMany(e => e.OrderItemDiscounts);
        await Converter.OrderItemDiscountRepository.AddAsync(orderItemDiscounts, cancellationToken);
        
        var dataModels = Converter.ConvertFromDomain(domainModels);
        await Dao.AddAsync(dataModels, cancellationToken);
    }
    
    public async Task UpdateAsync(IEnumerable<OrderItem> entities, CancellationToken cancellationToken = default) {
        var domainModels = entities.ToList();
        var orderItemDiscounts = domainModels.SelectMany(e => e.OrderItemDiscounts);
        await Converter.OrderItemDiscountRepository.UpdateAsync(orderItemDiscounts, cancellationToken);
        
        var dataModels = Converter.ConvertFromDomain(domainModels);
        await Dao.UpdateAsync(dataModels, cancellationToken);
    }
    
    private async Task<OrderItem> UpdateImplAsync(OrderItem entity, CancellationToken cancellationToken = default) {
        var dataModel = Converter.ConvertFromDomain(entity);
        dataModel = await Dao.UpdateAsync(dataModel, cancellationToken);
        entity = await Converter.ConvertToDomain(dataModel, cancellationToken);
        return entity;
    }

    public async Task UpdateOrderItemsAsync(IReadOnlyCollection<OrderItem> oldDomainModels, IReadOnlyCollection<OrderItem> newDomainModels,
        CancellationToken cancellationToken = default) {
        
        var discountsOfOldOrderItems = oldDomainModels.SelectMany(g => g.OrderItemDiscounts);
        var discountsOfNewOrderItems = newDomainModels.SelectMany(g => g.OrderItemDiscounts);
        await Converter.OrderItemDiscountRepository.UpdateDiscountsAsync(discountsOfOldOrderItems, discountsOfNewOrderItems, cancellationToken);

        var oldDataModels = oldDomainModels.Select(Converter.ConvertFromDomain);
        var newDataModels = newDomainModels.Select(Converter.ConvertFromDomain);
        await Dao.ApplyAsync(oldDataModels, newDataModels.ToImmutableArray(), cancellationToken);
    }

    public async Task DeleteAsync(IEnumerable<OrderItem> entities, CancellationToken cancellationToken = default) {
        var dataModels = Converter.ConvertFromDomain(entities);
        await Dao.DeleteAsync(dataModels, cancellationToken);
    }

    internal class OrderItemDomainModelConvert : IDomainReadModelConverter<ProductOrderDataModel, OrderItem> {
        public IEnumerable<OrderParameter>? DefaultOrderParameters { get; } = OrderParameter.From(nameof(ProductOrderDataModel.CreationTime));

        internal IProductRepository ProductRepository { get; }
        internal OrderItemDiscountRepository OrderItemDiscountRepository { get; }
        
        public OrderItemDomainModelConvert(IProductRepository productRepository, IDao<ProductOrderDiscountDataModel> orderItemDiscountDao) {
            OrderItemDiscountRepository = new OrderItemDiscountRepository(orderItemDiscountDao);
            ProductRepository = productRepository;
        }
        public async ValueTask<OrderItem> ConvertToDomain(ProductOrderDataModel dataModel, CancellationToken cancellationToken) {
            return (await ConvertToDomain(new List<ProductOrderDataModel>() { dataModel }, cancellationToken)).First();
        }
        public async Task<IReadOnlyList<OrderItem>> ConvertToDomain(IAsyncEnumerable<ProductOrderDataModel> dataModels,
            CancellationToken cancellationToken = default) {
            var items = await dataModels.ToListAsync(cancellationToken);
            return await ConvertToDomain(items, cancellationToken);
        }

        public async Task<IReadOnlyList<OrderItem>> ConvertToDomain(IReadOnlyCollection<ProductOrderDataModel> dataModels,
            CancellationToken cancellationToken = default) {
            var productIds = dataModels.Select(p => p.ProductId).ToHashSet();
            var productDict = (await ProductRepository
                    .FindByIdsAsync(productIds, cancellationToken))
                .ToDictionary(s => s.Id, s => s);

            var orderItemIds = dataModels.Select(p => p.Id).ToHashSet();
            var orderItemDiscountDict = (await OrderItemDiscountRepository
                .FindByOrderItemIds(orderItemIds, cancellationToken)).
                ToDictionary(s => s.Key, s => s);
            
            var domainModels = new List<OrderItem>();
            foreach (var dataModel in dataModels) {
                if (!productDict.TryGetValue(dataModel.ProductId, out var product)) {
                    throw new CaasDomainMappingException($"Failed to find product {dataModel.ProductId} for order-item {dataModel.Id}");
                }

                domainModels.Add(new OrderItem() {
                    Id = dataModel.Id,
                    ShopId = dataModel.ShopId,
                    OrderId = dataModel.OrderId,
                    Product = product,
                    Amount = dataModel.Amount,
                    PricePerPiece = product.Price,
                    OrderItemDiscounts =  orderItemDiscountDict.ContainsKey(dataModel.Id) ? 
                                            orderItemDiscountDict[dataModel.Id].Value.ToImmutableList() : ImmutableList<OrderItemDiscount>.Empty,
                    ConcurrencyToken = dataModel.GetConcurrencyToken()
                });
            }
            return domainModels;
        }

        public IReadOnlyList<ProductOrderDataModel> ConvertFromDomain(IEnumerable<OrderItem> domainModels)
            => domainModels.Select(ConvertFromDomain).ToList();

        public ProductOrderDataModel ConvertFromDomain(OrderItem domainModel) {
            return new ProductOrderDataModel() {
                Id = domainModel.Id,
                Amount = domainModel.Amount,
                ProductId = domainModel.Product.Id,
                OrderId = domainModel.OrderId,
                ShopId = domainModel.ShopId,
                PricePerPiece = domainModel.PricePerPiece,
                RowVersion = domainModel.GetRowVersion()
            };
        }
    }
}