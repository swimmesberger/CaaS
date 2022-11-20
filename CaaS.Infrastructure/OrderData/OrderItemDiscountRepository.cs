using System.Collections.Immutable;
using CaaS.Core.OrderAggregate;
using CaaS.Infrastructure.Base.Ado;
using CaaS.Infrastructure.Base.Ado.Model;
using CaaS.Infrastructure.Base.Repository;

namespace CaaS.Infrastructure.OrderData; 

internal class OrderItemDiscountRepository {
    private IDao<ProductOrderDiscountDataModel> Dao { get; }
    private OrderItemDiscountConvert Converter { get; }
    
    public OrderItemDiscountRepository(IDao<ProductOrderDiscountDataModel> dao) {
        Dao = dao;
        Converter = new OrderItemDiscountConvert();
    }
    
    public async Task<ItemDiscount?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) {
        var dataModel = await Dao.FindByIdAsync(id, cancellationToken);
        if (dataModel == null) return null;
        return await Converter.ConvertToDomain(dataModel, cancellationToken);
    }
    
    public async Task<IReadOnlyList<ItemDiscount>> FindByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default) {
        var dataModel = Dao.FindByIdsAsync(ids, cancellationToken);
        return await Converter.ConvertToDomain(dataModel, cancellationToken);
    }
    
    public async Task<IReadOnlyList<ItemDiscount>> FindByOrderItemId(Guid orderItemId, CancellationToken cancellationToken = default) {
        return (await Converter
                .ConvertToDomain(Dao
                    .FindBy(StatementParameters.CreateWhere(nameof(ProductOrderDiscountDataModel.ProductOrderId), orderItemId), cancellationToken), cancellationToken))
                .ToList();
    }
    
    public async Task<Dictionary<Guid, IReadOnlyList<ItemDiscount>>> FindByOrderItemIds(IEnumerable<Guid> orderItemIds,
        CancellationToken cancellationToken = default) {
        return (await Converter
                .ConvertToDomain(Dao
                    .FindBy(StatementParameters.CreateWhere(nameof(ProductOrderDiscountDataModel.ProductOrderId), orderItemIds), cancellationToken), cancellationToken))
            .GroupBy(i => i.ParentItemId)
            .ToDictionary(grp => grp.Key, grp => (IReadOnlyList<ItemDiscount>)grp.ToImmutableArray());
    }
    
    public async Task AddAsync(IEnumerable<ItemDiscount> entities, CancellationToken cancellationToken = default) {
        var dataModels = Converter.ConvertFromDomain(entities);
        await Dao.AddAsync(dataModels, cancellationToken);
    }
    
    public async Task UpdateAsync(IEnumerable<ItemDiscount> entities, CancellationToken cancellationToken = default) {
        var dataModels = Converter.ConvertFromDomain(entities);
        await Dao.UpdateAsync(dataModels, cancellationToken);
    }
    
    public async Task UpdateDiscountsAsync(IEnumerable<ItemDiscount> oldDomainModels, IEnumerable<ItemDiscount> newDomainModels,
        CancellationToken cancellationToken = default) {
        var oldDataModels = oldDomainModels.Select(Converter.ConvertFromDomain);
        var newDataModels = newDomainModels.Select(Converter.ConvertFromDomain);
        await Dao.ApplyAsync(oldDataModels, newDataModels.ToList(), cancellationToken);
    }
    
    public async Task DeleteAsync(IEnumerable<ItemDiscount> entities, CancellationToken cancellationToken = default) {
        var dataModels = Converter.ConvertFromDomain(entities);
        await Dao.DeleteAsync(dataModels, cancellationToken);
    }
    
    private class OrderItemDiscountConvert : IDomainModelConverter<ProductOrderDiscountDataModel, ItemDiscount> {
        public IEnumerable<OrderParameter>? DefaultOrderParameters => null;

        public async ValueTask<ItemDiscount> ConvertToDomain(ProductOrderDiscountDataModel dataModel, CancellationToken cancellationToken) {
            return (await ConvertToDomain(new List<ProductOrderDiscountDataModel>() { dataModel })).First();
        }
        
        public async Task<IReadOnlyList<ItemDiscount>> ConvertToDomain(IAsyncEnumerable<ProductOrderDiscountDataModel> dataModels, 
            CancellationToken cancellationToken = default) {
                var items = await dataModels.ToListAsync(cancellationToken);
                return await ConvertToDomain(items);
        }

        private Task<IReadOnlyList<ItemDiscount>> ConvertToDomain(IReadOnlyCollection<ProductOrderDiscountDataModel> dataModels) {
            var domainModels = dataModels.Select(dataModel => new ItemDiscount() {
                    Id = dataModel.Id,
                    DiscountName = dataModel.DiscountName,
                    DiscountValue = dataModel.Discount,
                    ParentItemId = dataModel.ProductOrderId,
                    ShopId = dataModel.ShopId,
                    ConcurrencyToken = dataModel.GetConcurrencyToken()
                })
                .ToList();
            return Task.FromResult<IReadOnlyList<ItemDiscount>>(domainModels);
        }
        
        public IReadOnlyList<ProductOrderDiscountDataModel> ConvertFromDomain(IEnumerable<ItemDiscount> domainModels)
            => domainModels.Select(ConvertFromDomain).ToList();
        
        public ProductOrderDiscountDataModel ConvertFromDomain(ItemDiscount domainModel) {
            return new ProductOrderDiscountDataModel {
                Id = domainModel.Id,
                DiscountName = domainModel.DiscountName,
                Discount = domainModel.DiscountValue,
                ProductOrderId = domainModel.ParentItemId,
                ShopId = domainModel.ShopId,
                RowVersion = domainModel.GetRowVersion(),
            };
        }
        
        public ProductOrderDiscountDataModel ApplyDomainModel(ProductOrderDiscountDataModel dataModel, ItemDiscount domainModel) {
            return dataModel with {
                Id = domainModel.Id,
                DiscountName = domainModel.DiscountName,
                Discount = domainModel.DiscountValue,
                ProductOrderId = domainModel.ParentItemId,
                ShopId = domainModel.ShopId,
                RowVersion = domainModel.GetRowVersion(),
            };
        }
    }
}