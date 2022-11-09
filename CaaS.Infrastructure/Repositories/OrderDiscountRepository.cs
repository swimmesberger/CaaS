using System.Collections.Immutable;
using CaaS.Core.Entities;
using CaaS.Infrastructure.Ado.Base;
using CaaS.Infrastructure.Ado.Model;
using CaaS.Infrastructure.DataModel;
using CaaS.Infrastructure.Repositories.Base;

namespace CaaS.Infrastructure.Repositories; 

internal class OrderDiscountRepository {
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
           .ToImmutableList();
   }
   
   public async Task<Dictionary<Guid, IReadOnlyList<OrderDiscount>>> FindByOrderIdsAsync(IEnumerable<Guid> orderIds,
       CancellationToken cancellationToken = default) {
       return (await Converter
               .ConvertToDomain(Dao
                   .FindBy(StatementParameters.CreateWhere(nameof(OrderDataModel.Id), orderIds), cancellationToken), cancellationToken))
           .GroupBy(i => i.OrderId)
           .ToDictionary(grp => grp.Key, grp => (IReadOnlyList<OrderDiscount>)grp.ToImmutableList());
   }

   internal class OrderDiscountDomainModelConvert : IDomainModelConverter<OrderDiscountDataModel, OrderDiscount> {
       public IEnumerable<OrderParameter>? DefaultOrderParameters { get; } = null;
       public async ValueTask<OrderDiscount> ConvertToDomain(OrderDiscountDataModel dataModel, CancellationToken cancellationToken) {
           return (await ConvertToDomain(new List<OrderDiscountDataModel>() { dataModel }, cancellationToken)).First();
       }
       public async  Task<IReadOnlyList<OrderDiscount>> ConvertToDomain(IAsyncEnumerable<OrderDiscountDataModel> dataModels, CancellationToken cancellationToken = default) {
           var items = await dataModels.ToListAsync(cancellationToken);
           return await ConvertToDomain(items, cancellationToken);
       }
       
       public Task<List<OrderDiscount>> ConvertToDomain(IReadOnlyCollection<OrderDiscountDataModel> dataModels, CancellationToken cancellationToken = default) {
          
           var domainModels = new List<OrderDiscount>();
           foreach (var dataModel in dataModels) {
               domainModels.Add(new OrderDiscount() {
                   Id = dataModel.Id,
                   ShopId = dataModel.ShopId,
                   DiscountName = dataModel.DiscountName,
                   DiscountValue = dataModel.Discount,
                   OrderId = dataModel.OrderId,
                   ConcurrencyToken = dataModel.GetConcurrencyToken()
               });
           }
           return Task.FromResult(domainModels);
       }
       
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