using System.Collections.Immutable;
using CaaS.Core.Entities;
using CaaS.Core.Repositories.Base;
using CaaS.Infrastructure.Ado.Base;
using CaaS.Infrastructure.Ado.Model;
using CaaS.Infrastructure.DataModel;
using CaaS.Infrastructure.Repositories.Base;

namespace CaaS.Infrastructure.Repositories; 

internal class OrderDiscountRepository : IRepository {
   private IDao<OrderDiscountDataModel> Dao { get; }
   private OrderDiscountDomainModelConvert Converter { get;  }

   public OrderDiscountRepository(IDao<OrderDiscountDataModel> dao) {
       Dao = dao;
       Converter = new OrderDiscountDomainModelConvert();
   }
   
   public async Task<IReadOnlyList<OrderDiscount>> FindByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default) {
       var dataModel = Dao.FindByIdsAsync(ids, cancellationToken);
       return await Converter.ConvertToDomain(dataModel, cancellationToken);
   }
   
   public async Task<IReadOnlyList<OrderDiscount>> FindByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default) {
       return (await Converter.ConvertToDomain(Dao
               .FindBy(StatementParameters.CreateWhere(nameof(OrderDiscount.OrderId), orderId), cancellationToken), cancellationToken))
           .ToList();
   }
   
   public async Task<OrderDiscount> AddAsync(OrderDiscount entity, CancellationToken cancellationToken = default) {
       var dataModel = Converter.ConvertFromDomain(entity); 
       dataModel = await Dao.AddAsync(dataModel, cancellationToken);
       entity = await Converter.ConvertToDomain(dataModel, cancellationToken);
       return entity;
   }
   
   public async Task<OrderDiscount> UpdateAsync(OrderDiscount entity, CancellationToken cancellationToken = default) {
       var dataModel = Converter.ConvertFromDomain(entity);
       dataModel = await Dao.UpdateAsync(dataModel, cancellationToken);
       entity = await Converter.ConvertToDomain(dataModel, cancellationToken);
       return entity;
   }
   
   public async Task DeleteAsync(OrderDiscount entity, CancellationToken cancellationToken = default) {
       await Dao.DeleteAsync(Converter.ConvertFromDomain(entity), cancellationToken);
   }
   
   public async Task AddAsync(IEnumerable<OrderDiscount> entities, CancellationToken cancellationToken = default) {
       var dataModels = Converter.ConvertFromDomain(entities);
       await Dao.AddAsync(dataModels, cancellationToken);
   }

   public async Task UpdateAsync(IEnumerable<OrderDiscount> entities, CancellationToken cancellationToken = default) {
       var dataModels = Converter.ConvertFromDomain(entities);
       await Dao.UpdateAsync(dataModels, cancellationToken);
   }

   public async Task DeleteAsync(IEnumerable<OrderDiscount> entities, CancellationToken cancellationToken = default) {
       var dataModels = Converter.ConvertFromDomain(entities);
       await Dao.DeleteAsync(dataModels, cancellationToken);
   }
   
   public async Task<Dictionary<Guid, IReadOnlyList<OrderDiscount>>> FindByOrderIdsAsync(IEnumerable<Guid> orderIds,
       CancellationToken cancellationToken = default) {
       return (await Converter
               .ConvertToDomain(Dao
                   .FindBy(StatementParameters.CreateWhere(nameof(OrderDiscount.OrderId), orderIds), cancellationToken), cancellationToken))
           .GroupBy(i => i.OrderId)
           .ToDictionary(grp => grp.Key, grp => (IReadOnlyList<OrderDiscount>)grp.ToImmutableArray());
   }

   private class OrderDiscountDomainModelConvert : IDomainModelConverter<OrderDiscountDataModel, OrderDiscount> {
       public IEnumerable<OrderParameter>? DefaultOrderParameters => null;
       
       public ValueTask<OrderDiscount> ConvertToDomain(OrderDiscountDataModel dataModel, CancellationToken cancellationToken) {
           return new ValueTask<OrderDiscount>(
               ConvertToDomain(new List<OrderDiscountDataModel> { dataModel }).First()
           );
       }
       
       public async Task<IReadOnlyList<OrderDiscount>> ConvertToDomain(IAsyncEnumerable<OrderDiscountDataModel> dataModels, CancellationToken cancellationToken = default) {
           var items = await dataModels.ToListAsync(cancellationToken);
           return ConvertToDomain(items);
       }

       private IReadOnlyList<OrderDiscount> ConvertToDomain(IEnumerable<OrderDiscountDataModel> dataModels) {
           return dataModels.Select(dataModel => new OrderDiscount() {
                   Id = dataModel.Id,
                   ShopId = dataModel.ShopId,
                   DiscountName = dataModel.DiscountName,
                   DiscountValue = dataModel.Discount,
                   OrderId = dataModel.OrderId,
                   ConcurrencyToken = dataModel.GetConcurrencyToken()
               })
               .ToList();
       }
       
       public IReadOnlyList<OrderDiscountDataModel> ConvertFromDomain(IEnumerable<OrderDiscount> domainModels)
           => domainModels.Select(ConvertFromDomain).ToList();
       
       public OrderDiscountDataModel ConvertFromDomain(OrderDiscount domainModel) {
           return new OrderDiscountDataModel {
               Id = domainModel.Id,
               DiscountName = domainModel.DiscountName,
               Discount = domainModel.DiscountValue,
               OrderId = domainModel.OrderId,
               ShopId = domainModel.ShopId,
               RowVersion = domainModel.GetRowVersion(),
           };
       }
       
       public OrderDiscountDataModel ApplyDomainModel(OrderDiscountDataModel dataModel, OrderDiscount domainModel) {
           return dataModel with {
               Id = domainModel.Id,
               DiscountName = domainModel.DiscountName,
               Discount = domainModel.DiscountValue,
               OrderId = domainModel.OrderId,
               ShopId = domainModel.ShopId,
               RowVersion = domainModel.GetRowVersion(),
           };
       }
   }
}