using System.Collections.Immutable;
using CaaS.Core.CouponAggregate;
using CaaS.Infrastructure.Base.Ado;
using CaaS.Infrastructure.Base.Ado.Model;
using CaaS.Infrastructure.Base.Repository;

namespace CaaS.Infrastructure.CouponData; 

public class CouponRepository : CrudReadRepository<CouponDataModel, Coupon>, ICouponRepository {
    internal new CouponDomainModelConvert Converter => (CouponDomainModelConvert)base.Converter;

    public CouponRepository(IDao<CouponDataModel> dao): 
                            base(dao, new CouponDomainModelConvert()) { }
    
    public async Task<Dictionary<Guid, IReadOnlyList<Coupon>>> FindByOrderIds(IEnumerable<Guid> orderIds,
        CancellationToken cancellationToken = default) {
        return (await Converter
                .ConvertToDomain(Dao
                    .FindBy(StatementParameters.CreateWhere(nameof(CouponDataModel.OrderId), orderIds), cancellationToken), cancellationToken))
            .GroupBy(i => i.OrderId!.Value)
            .ToDictionary(grp => grp.Key, grp => (IReadOnlyList<Coupon>)grp.ToList());
    }
    
    public async Task<IReadOnlyList<Coupon>> FindByCustomerId(Guid customerId, CancellationToken cancellationToken = default) {
        return (await Converter.ConvertToDomain(Dao
                .FindBy(StatementParameters.CreateWhere(nameof(Coupon.CustomerId), customerId), cancellationToken), cancellationToken))
            .ToList();
    }
    public async Task<IReadOnlyList<Coupon>> FindByOrderId(Guid orderId, CancellationToken cancellationToken = default) {
        return (await Converter.ConvertToDomain(Dao
                .FindBy(StatementParameters.CreateWhere(nameof(Coupon.OrderId), orderId), cancellationToken), cancellationToken))
            .ToList();
    }
    public async Task<IReadOnlyList<Coupon>> FindByCartId(Guid cartId, CancellationToken cancellationToken = default) {
        return (await Converter.ConvertToDomain(Dao
                .FindBy(StatementParameters.CreateWhere(nameof(Coupon.CartId), cartId), cancellationToken), cancellationToken))
            .ToList();
    }
    
    public async Task<Coupon> AddAsync(Coupon entity, CancellationToken cancellationToken = default) {
        var dataModel = Converter.ConvertFromDomain(entity);
        await Dao.AddAsync(dataModel, cancellationToken);
        entity = Converter.ApplyDataModel(entity, dataModel);
        return entity;
    }
    
    public async Task AddAsync(IEnumerable<Coupon> entities, CancellationToken cancellationToken = default) {
        var dataModels = Converter.ConvertFromDomain(entities);
        await Dao.AddAsync(dataModels, cancellationToken);
    }
    
    public async Task<Coupon> UpdateAsync(Coupon oldEntity, Coupon newEntity, CancellationToken cancellationToken = default) {
        var dataModel = Converter.ConvertFromDomain(newEntity);
        dataModel = await Dao.UpdateAsync(dataModel, cancellationToken);
        newEntity = Converter.ApplyDataModel(newEntity, dataModel);
        return newEntity;
    }

    public async Task UpdateAsync(IEnumerable<Coupon> oldDomainModels, IEnumerable<Coupon> newDomainModels, CancellationToken cancellationToken = default) {
        var oldDataModels = oldDomainModels.Select(Converter.ConvertFromDomain);
        var newDataModels = newDomainModels.Select(Converter.ConvertFromDomain);
        await Dao.ApplyAsync(oldDataModels, newDataModels.ToList(), cancellationToken);
    }

    public async Task DeleteAsync(Coupon entity, CancellationToken cancellationToken = default) {
        var dataModel = Converter.ConvertFromDomain(entity);
        await Dao.DeleteAsync(dataModel, cancellationToken);
    }
}

internal class CouponDomainModelConvert : IDomainReadModelConverter<CouponDataModel, Coupon> {
    public IEnumerable<OrderParameter>? DefaultOrderParameters { get; } = null;
    
    public Coupon ApplyDataModel(Coupon domainModel, CouponDataModel dataModel) {
        return domainModel with { ConcurrencyToken = dataModel.GetConcurrencyToken() };
    }
    
    public IReadOnlyList<CouponDataModel> ConvertFromDomain(IEnumerable<Coupon> domainModels)
        => domainModels.Select(ConvertFromDomain).ToImmutableArray();

    public CouponDataModel ConvertFromDomain(Coupon domainModel) {
        return new CouponDataModel {
            Id = domainModel.Id,
            ShopId = domainModel.ShopId,
            Value = domainModel.Value,
            OrderId = domainModel.OrderId,
            CartId = domainModel.CartId,
            RedeemedBy = domainModel.CustomerId,
            RowVersion = domainModel.GetRowVersion(),
        };
    }
    public ValueTask<Coupon> ConvertToDomain(CouponDataModel dataModel, CancellationToken cancellationToken) {
        return ValueTask.FromResult(ConvertToDomain(new List<CouponDataModel> { dataModel })[0]);
    }
    
    public async Task<IReadOnlyList<Coupon>> ConvertToDomain(IAsyncEnumerable<CouponDataModel> dataModels, CancellationToken cancellationToken = default) {
        var items = await dataModels.ToListAsync(cancellationToken);
        return ConvertToDomain(items);
    }
    
    private IReadOnlyList<Coupon> ConvertToDomain(IEnumerable<CouponDataModel> dataModels) {
        return dataModels.Select(dataModel => new Coupon() {
                Id = dataModel.Id,
                ShopId = dataModel.ShopId,
                Value = dataModel.Value,
                OrderId = dataModel.OrderId,
                CartId = dataModel.CartId,
                CustomerId = dataModel.RedeemedBy,
                ConcurrencyToken = dataModel.GetConcurrencyToken()
            })
            .ToList();
    }
}