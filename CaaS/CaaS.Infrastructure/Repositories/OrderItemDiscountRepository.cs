﻿using System.Collections.Immutable;
using CaaS.Core.Discount;
using CaaS.Core.Entities;
using CaaS.Infrastructure.Ado.Base;
using CaaS.Infrastructure.Ado.Model;
using CaaS.Infrastructure.DataModel;
using CaaS.Infrastructure.Repositories.Base;

namespace CaaS.Infrastructure.Repositories; 

internal class OrderItemDiscountRepository {
    private IDao<ProductOrderDiscountDataModel> Dao { get; }
    private OrderItemDiscountConvert Converter { get; }
    
    public OrderItemDiscountRepository(IDao<ProductOrderDiscountDataModel> dao) {
        Dao = dao;
        Converter = new OrderItemDiscountConvert();
    }
    
    public async Task<OrderItemDiscount?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) {
        var dataModel = await Dao.FindByIdAsync(id, cancellationToken);
        if (dataModel == null) return null;
        return await Converter.ConvertToDomain(dataModel, cancellationToken);
    }
    
    public async Task<IReadOnlyList<OrderItemDiscount>> FindByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default) {
        var dataModel = Dao.FindByIdsAsync(ids, cancellationToken);
        return await Converter.ConvertToDomain(dataModel, cancellationToken);
    }
    
    public async Task<IReadOnlyList<OrderItemDiscount>> FindByOrderItemId(Guid orderItemId, CancellationToken cancellationToken = default) {
        return (await Converter
                .ConvertToDomain(Dao
                    .FindBy(StatementParameters.CreateWhere(nameof(ProductOrderDiscountDataModel.ProductOrderId), orderItemId), cancellationToken), cancellationToken))
                .ToImmutableList();
    }
    
    public async Task<Dictionary<Guid, IReadOnlyList<OrderItemDiscount>>> FindByOrderItemIds(IEnumerable<Guid> orderItemIds,
        CancellationToken cancellationToken = default) {
        return (await Converter
                .ConvertToDomain(Dao
                    .FindBy(StatementParameters.CreateWhere(nameof(ProductOrderDiscountDataModel.ProductOrderId), orderItemIds), cancellationToken), cancellationToken))
            .GroupBy(i => i.OrderItemId)
            .ToDictionary(grp => grp.Key, grp => (IReadOnlyList<OrderItemDiscount>)grp.ToImmutableList());
    }
    
    public async Task AddAsync(IEnumerable<OrderItemDiscount> entities, CancellationToken cancellationToken = default) {
        var dataModels = Converter.ConvertFromDomain(entities);
        await Dao.AddAsync(dataModels, cancellationToken);
    }
    
    public async Task UpdateAsync(IEnumerable<OrderItemDiscount> entities, CancellationToken cancellationToken = default) {
        var dataModels = Converter.ConvertFromDomain(entities);
        await Dao.UpdateAsync(dataModels, cancellationToken);
    }
    
    public async Task UpdateDiscountsAsync(IEnumerable<OrderItemDiscount> oldDomainModels, IEnumerable<OrderItemDiscount> newDomainModels,
        CancellationToken cancellationToken = default) {
        var oldDataModels = oldDomainModels.Select(Converter.ConvertFromDomain);
        var newDataModels = newDomainModels.Select(Converter.ConvertFromDomain);
        await Dao.ApplyAsync(oldDataModels, newDataModels.ToImmutableArray(), cancellationToken);
    }
    
    public async Task DeleteAsync(IEnumerable<OrderItemDiscount> entities, CancellationToken cancellationToken = default) {
        var dataModels = Converter.ConvertFromDomain(entities);
        await Dao.DeleteAsync(dataModels, cancellationToken);
    }
    
    private class OrderItemDiscountConvert : IDomainModelConverter<ProductOrderDiscountDataModel, OrderItemDiscount> {
        public IEnumerable<OrderParameter>? DefaultOrderParameters { get; } = null;
        
        public OrderItemDiscountConvert() {}
        public async ValueTask<OrderItemDiscount> ConvertToDomain(ProductOrderDiscountDataModel dataModel, CancellationToken cancellationToken) {
            return (await ConvertToDomain(new List<ProductOrderDiscountDataModel>() { dataModel }, cancellationToken)).First();
        }
        public async Task<IReadOnlyList<OrderItemDiscount>> ConvertToDomain(IAsyncEnumerable<ProductOrderDiscountDataModel> dataModels, 
            CancellationToken cancellationToken = default) {
                var items = await dataModels.ToListAsync(cancellationToken);
                return await ConvertToDomain(items, cancellationToken);
        }
        
        public Task<IReadOnlyList<OrderItemDiscount>> ConvertToDomain(IReadOnlyCollection<ProductOrderDiscountDataModel> dataModels,
            CancellationToken cancellationToken = default) {
            
            var domainModels = new List<OrderItemDiscount>();
            foreach (var dataModel in dataModels) {
                domainModels.Add(new OrderItemDiscount() {
                    Id = dataModel.Id,
                    DiscountName = dataModel.DiscountName,
                    DiscountValue = dataModel.Discount,
                    OrderItemId = dataModel.ProductOrderId,
                    ShopId = dataModel.ShopId,
                    ConcurrencyToken = dataModel.GetConcurrencyToken()
                });
            }
            return Task.FromResult<IReadOnlyList<OrderItemDiscount>>(domainModels);
        }
        
        public IReadOnlyList<ProductOrderDiscountDataModel> ConvertFromDomain(IEnumerable<OrderItemDiscount> domainModels)
            => domainModels.Select(ConvertFromDomain).ToList();
        
        public ProductOrderDiscountDataModel ConvertFromDomain(OrderItemDiscount domainModel) {
            return new ProductOrderDiscountDataModel {
                Id = domainModel.Id,
                DiscountName = domainModel.DiscountName,
                Discount = domainModel.DiscountValue,
                ProductOrderId = domainModel.OrderItemId,
                ShopId = domainModel.ShopId,
                RowVersion = domainModel.GetRowVersion(),
            };
        }
        
        public ProductOrderDiscountDataModel ApplyDomainModel(ProductOrderDiscountDataModel dataModel, OrderItemDiscount domainModel) {
            return dataModel with {
                Id = domainModel.Id,
                DiscountName = domainModel.DiscountName,
                Discount = domainModel.DiscountValue,
                ProductOrderId = domainModel.OrderItemId,
                ShopId = domainModel.ShopId,
                RowVersion = domainModel.GetRowVersion(),
            };
        }
    }
}