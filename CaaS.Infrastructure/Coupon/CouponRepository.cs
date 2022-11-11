using System.Collections.Immutable;
using CaaS.Core.Coupon;
using CaaS.Infrastructure.Base.Ado;
using CaaS.Infrastructure.Base.Ado.Model;
using CaaS.Infrastructure.Base.Repository;

namespace CaaS.Infrastructure.Coupon; 

public class CouponRepository : CrudReadRepository<CouponDataModel, Core.Coupon.Entities.Coupon>, ICouponRepository {
    internal new CouponDomainModelConvert Converter => (CouponDomainModelConvert)base.Converter;

    public CouponRepository(IDao<CouponDataModel> dao): 
                            base(dao, new CouponDomainModelConvert()) { }
    
    public async Task<Dictionary<Guid, IReadOnlyList<Core.Coupon.Entities.Coupon>>> FindByOrderIds(IEnumerable<Guid> orderIds,
        CancellationToken cancellationToken = default) {
        return (await Converter
                .ConvertToDomain(Dao
                    .FindBy(StatementParameters.CreateWhere(nameof(CouponDataModel.OrderId), orderIds), cancellationToken), cancellationToken))
            .GroupBy(i => i.OrderId!.Value)
            .ToDictionary(grp => grp.Key, grp => (IReadOnlyList<Core.Coupon.Entities.Coupon>)grp.ToList());
    }
    
    public async Task<IReadOnlyList<Core.Coupon.Entities.Coupon>> FindCouponsByCustomerId(Guid customerId, CancellationToken cancellationToken = default) {
        return (await Converter.ConvertToDomain(Dao
                .FindBy(StatementParameters.CreateWhere(nameof(Core.Coupon.Entities.Coupon.CustomerId), customerId), cancellationToken), cancellationToken))
            .ToList();
    }
    public async Task<IReadOnlyList<Core.Coupon.Entities.Coupon>> FindCouponsByOrderId(Guid orderId, CancellationToken cancellationToken = default) {
        return (await Converter.ConvertToDomain(Dao
                .FindBy(StatementParameters.CreateWhere(nameof(Core.Coupon.Entities.Coupon.OrderId), orderId), cancellationToken), cancellationToken))
            .ToList();
    }
    public async Task<IReadOnlyList<Core.Coupon.Entities.Coupon>> FindCouponsByCartId(Guid cartId, CancellationToken cancellationToken = default) {
        return (await Converter.ConvertToDomain(Dao
                .FindBy(StatementParameters.CreateWhere(nameof(Core.Coupon.Entities.Coupon.CartId), cartId), cancellationToken), cancellationToken))
            .ToList();
    }
    
    public async Task<Core.Coupon.Entities.Coupon> AddAsync(Core.Coupon.Entities.Coupon entity, CancellationToken cancellationToken = default) {
        var dataModel = Converter.ConvertFromDomain(entity);
        await Dao.AddAsync(dataModel, cancellationToken);
        entity = await Converter.ConvertToDomain(dataModel, cancellationToken);
        return entity;
    }
    
    public async Task AddAsync(IEnumerable<Core.Coupon.Entities.Coupon> entities, CancellationToken cancellationToken = default) {
        var dataModels = Converter.ConvertFromDomain(entities);
        await Dao.AddAsync(dataModels, cancellationToken);
    }
    
    public async Task<Core.Coupon.Entities.Coupon> UpdateAsync(Core.Coupon.Entities.Coupon oldEntity, Core.Coupon.Entities.Coupon newEntity, CancellationToken cancellationToken = default) {
        return await UpdateImplAsync(newEntity, cancellationToken);
    }

    public async Task UpdateAsync(IEnumerable<Core.Coupon.Entities.Coupon> oldDomainModels, IEnumerable<Core.Coupon.Entities.Coupon> newDomainModels, CancellationToken cancellationToken = default) {
        var oldDataModels = oldDomainModels.Select(Converter.ConvertFromDomain);
        var newDataModels = newDomainModels.Select(Converter.ConvertFromDomain);
        await Dao.ApplyAsync(oldDataModels, newDataModels.ToList(), cancellationToken);
    }

    private async Task<Core.Coupon.Entities.Coupon> UpdateImplAsync(Core.Coupon.Entities.Coupon entity, CancellationToken cancellationToken = default) {
        var dataModel = Converter.ConvertFromDomain(entity);
        dataModel = await Dao.UpdateAsync(dataModel, cancellationToken);
        entity = await Converter.ConvertToDomain(dataModel, cancellationToken);
        return entity;
    }

    public async Task DeleteAsync(Core.Coupon.Entities.Coupon entity, CancellationToken cancellationToken = default) {
        var dataModel = Converter.ConvertFromDomain(entity);
        await Dao.DeleteAsync(dataModel, cancellationToken);
    }
}

internal class CouponDomainModelConvert : IDomainReadModelConverter<CouponDataModel, Core.Coupon.Entities.Coupon> {
    public IEnumerable<OrderParameter>? DefaultOrderParameters { get; } = null;
    
    public IReadOnlyList<CouponDataModel> ConvertFromDomain(IEnumerable<Core.Coupon.Entities.Coupon> domainModels)
        => domainModels.Select(ConvertFromDomain).ToImmutableArray();

    public CouponDataModel ConvertFromDomain(Core.Coupon.Entities.Coupon domainModel) {
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
    public ValueTask<Core.Coupon.Entities.Coupon> ConvertToDomain(CouponDataModel dataModel, CancellationToken cancellationToken) {
        return ValueTask.FromResult(ConvertToDomain(new List<CouponDataModel> { dataModel }).First());
    }
    public async Task<IReadOnlyList<Core.Coupon.Entities.Coupon>> ConvertToDomain(IAsyncEnumerable<CouponDataModel> dataModels, CancellationToken cancellationToken = default) {
        var items = await dataModels.ToListAsync(cancellationToken);
        return ConvertToDomain(items);
    }
    
    private IReadOnlyList<Core.Coupon.Entities.Coupon> ConvertToDomain(IEnumerable<CouponDataModel> dataModels) {
        return dataModels.Select(dataModel => new Core.Coupon.Entities.Coupon() {
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