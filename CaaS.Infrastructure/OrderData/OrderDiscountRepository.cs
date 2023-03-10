using System.Collections.Immutable;
using CaaS.Core.Base;
using CaaS.Core.DiscountAggregate.Models;
using CaaS.Infrastructure.Base.Ado;
using CaaS.Infrastructure.Base.Ado.Query.Parameters;
using CaaS.Infrastructure.Base.Repository;

namespace CaaS.Infrastructure.OrderData; 

internal class OrderDiscountRepository : IRepository {
   private IDao<OrderDiscountDataModel> Dao { get; }
   private OrderDiscountDomainModelConvert Converter { get;  }

   public OrderDiscountRepository(IDao<OrderDiscountDataModel> dao) {
       Dao = dao;
       Converter = new OrderDiscountDomainModelConvert();
   }
   
   public async Task UpdateAsync(IEnumerable<Discount> oldDomainModels, IEnumerable<Discount> newDomainModels, CancellationToken cancellationToken = default) {
       var oldDataModels = oldDomainModels.Select(Converter.ConvertFromDomain);
       var newDataModels = newDomainModels.Select(Converter.ConvertFromDomain);
       await Dao.ApplyAsync(oldDataModels, newDataModels.ToList(), cancellationToken);
   }
   
   public async Task AddAsync(IEnumerable<Discount> entities, CancellationToken cancellationToken = default) {
       var dataModels = Converter.ConvertFromDomain(entities);
       await Dao.AddAsync(dataModels, cancellationToken);
   }
   
   public async Task<Dictionary<Guid, IReadOnlyList<Discount>>> FindByOrderIdsAsync(IEnumerable<Guid> orderIds,
       CancellationToken cancellationToken = default) {
       return (await Converter
               .ConvertToDomain(Dao
                   .FindBy(StatementParameters.CreateWhere(nameof(OrderDiscountDataModel.OrderId), orderIds), cancellationToken), cancellationToken))
           .GroupBy(i => i.ParentId)
           .ToDictionary(grp => grp.Key, grp => (IReadOnlyList<Discount>)grp.ToImmutableArray());
   }

   private class OrderDiscountDomainModelConvert : IDomainModelConverter<OrderDiscountDataModel, Discount> {
       public IEnumerable<OrderParameter>? DefaultOrderParameters => null;
       
       public ValueTask<Discount> ConvertToDomain(OrderDiscountDataModel dataModel, CancellationToken cancellationToken) {
           return new ValueTask<Discount>(
               ConvertToDomain(new List<OrderDiscountDataModel> { dataModel }).First()
           );
       }
       
       public async Task<IReadOnlyList<Discount>> ConvertToDomain(IAsyncEnumerable<OrderDiscountDataModel> dataModels, CancellationToken cancellationToken = default) {
           var items = await dataModels.ToListAsync(cancellationToken);
           return ConvertToDomain(items);
       }

       private IReadOnlyList<Discount> ConvertToDomain(IEnumerable<OrderDiscountDataModel> dataModels) {
           return dataModels.Select(dataModel => new Discount() {
                   Id = dataModel.Id,
                   ShopId = dataModel.ShopId,
                   DiscountName = dataModel.DiscountName,
                   DiscountValue = dataModel.Discount,
                   ParentId = dataModel.OrderId,
                   ConcurrencyToken = dataModel.GetConcurrencyToken()
               })
               .ToList();
       }
       
       public IReadOnlyList<OrderDiscountDataModel> ConvertFromDomain(IEnumerable<Discount> domainModels)
           => domainModels.Select(ConvertFromDomain).ToList();
       
       public OrderDiscountDataModel ConvertFromDomain(Discount domainModel) {
           return new OrderDiscountDataModel {
               Id = domainModel.Id,
               DiscountName = domainModel.DiscountName,
               Discount = domainModel.DiscountValue,
               OrderId = domainModel.ParentId,
               ShopId = domainModel.ShopId,
               RowVersion = domainModel.GetRowVersion(),
           };
       }
       
       public OrderDiscountDataModel ApplyDomainModel(OrderDiscountDataModel dataModel, Discount domainModel) {
           return dataModel with {
               Id = domainModel.Id,
               DiscountName = domainModel.DiscountName,
               Discount = domainModel.DiscountValue,
               OrderId = domainModel.ParentId,
               ShopId = domainModel.ShopId,
               RowVersion = domainModel.GetRowVersion(),
           };
       }

       public Discount ApplyDataModel(Discount domainModel, OrderDiscountDataModel dataModel) {
           return domainModel with { ConcurrencyToken = dataModel.GetConcurrencyToken() };
       }
   }
}