using System.Collections.Immutable;
using CaaS.Core.DiscountAggregate.Models;
using CaaS.Infrastructure.Base.Ado;
using CaaS.Infrastructure.Base.Ado.Query.Parameters;
using CaaS.Infrastructure.Base.Repository;

namespace CaaS.Infrastructure.OrderData; 

internal class OrderItemDiscountRepository {
    private IDao<ProductOrderDiscountDataModel> Dao { get; }
    private OrderDiscountConvert Converter { get; }
    
    public OrderItemDiscountRepository(IDao<ProductOrderDiscountDataModel> dao) {
        Dao = dao;
        Converter = new OrderDiscountConvert();
    }
    
    public async Task<Dictionary<Guid, IReadOnlyList<Discount>>> FindByOrderItemIds(IEnumerable<Guid> orderItemIds,
        CancellationToken cancellationToken = default) {
        return (await Converter
                .ConvertToDomain(Dao
                    .FindBy(StatementParameters.CreateWhere(nameof(ProductOrderDiscountDataModel.ProductOrderId), orderItemIds), cancellationToken), cancellationToken))
            .GroupBy(i => i.ParentId)
            .ToDictionary(grp => grp.Key, grp => (IReadOnlyList<Discount>)grp.ToImmutableArray());
    }
    
    public async Task AddAsync(IEnumerable<Discount> entities, CancellationToken cancellationToken = default) {
        var dataModels = Converter.ConvertFromDomain(entities);
        await Dao.AddAsync(dataModels, cancellationToken);
    }
    
    public async Task UpdateAsync(IEnumerable<Discount> entities, CancellationToken cancellationToken = default) {
        var dataModels = Converter.ConvertFromDomain(entities);
        await Dao.UpdateAsync(dataModels, cancellationToken);
    }
    
    public async Task UpdateDiscountsAsync(IEnumerable<Discount> oldDomainModels, IEnumerable<Discount> newDomainModels,
        CancellationToken cancellationToken = default) {
        var oldDataModels = oldDomainModels.Select(Converter.ConvertFromDomain);
        var newDataModels = newDomainModels.Select(Converter.ConvertFromDomain);
        await Dao.ApplyAsync(oldDataModels, newDataModels.ToList(), cancellationToken);
    }
    
    private class OrderDiscountConvert : IDomainModelConverter<ProductOrderDiscountDataModel, Discount> {
        public IEnumerable<OrderParameter>? DefaultOrderParameters => null;

        public async ValueTask<Discount> ConvertToDomain(ProductOrderDiscountDataModel dataModel, CancellationToken cancellationToken) {
            return (await ConvertToDomain(new List<ProductOrderDiscountDataModel>() { dataModel })).First();
        }
        
        public async Task<IReadOnlyList<Discount>> ConvertToDomain(IAsyncEnumerable<ProductOrderDiscountDataModel> dataModels, 
            CancellationToken cancellationToken = default) {
                var items = await dataModels.ToListAsync(cancellationToken);
                return await ConvertToDomain(items);
        }

        private Task<IReadOnlyList<Discount>> ConvertToDomain(IReadOnlyCollection<ProductOrderDiscountDataModel> dataModels) {
            var domainModels = dataModels.Select(dataModel => new Discount() {
                    Id = dataModel.Id,
                    DiscountName = dataModel.DiscountName,
                    DiscountValue = dataModel.Discount,
                    ParentId = dataModel.ProductOrderId,
                    ShopId = dataModel.ShopId,
                    ConcurrencyToken = dataModel.GetConcurrencyToken()
                })
                .ToList();
            return Task.FromResult<IReadOnlyList<Discount>>(domainModels);
        }
        
        public IReadOnlyList<ProductOrderDiscountDataModel> ConvertFromDomain(IEnumerable<Discount> domainModels)
            => domainModels.Select(ConvertFromDomain).ToList();
        
        public ProductOrderDiscountDataModel ConvertFromDomain(Discount domainModel) {
            return new ProductOrderDiscountDataModel {
                Id = domainModel.Id,
                DiscountName = domainModel.DiscountName,
                Discount = domainModel.DiscountValue,
                ProductOrderId = domainModel.ParentId,
                ShopId = domainModel.ShopId,
                RowVersion = domainModel.GetRowVersion(),
            };
        }
        
        public ProductOrderDiscountDataModel ApplyDomainModel(ProductOrderDiscountDataModel dataModel, Discount domainModel) {
            return dataModel with {
                Id = domainModel.Id,
                DiscountName = domainModel.DiscountName,
                Discount = domainModel.DiscountValue,
                ProductOrderId = domainModel.ParentId,
                ShopId = domainModel.ShopId,
                RowVersion = domainModel.GetRowVersion(),
            };
        }

        public Discount ApplyDataModel(Discount domainModel, ProductOrderDiscountDataModel dataModel) {
            return domainModel with { ConcurrencyToken = dataModel.GetConcurrencyToken() };
        }
    }
}