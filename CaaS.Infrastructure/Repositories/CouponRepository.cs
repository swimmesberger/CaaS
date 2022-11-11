using System.Collections.Immutable;
using CaaS.Core.Entities;
using CaaS.Core.Repositories;
using CaaS.Core.Repositories.Base;
using CaaS.Infrastructure.Ado.Base;
using CaaS.Infrastructure.Ado.Model;
using CaaS.Infrastructure.DataModel;
using CaaS.Infrastructure.Repositories.Base;

namespace CaaS.Infrastructure.Repositories; 

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
            .ToDictionary(grp => grp.Key, grp => (IReadOnlyList<Coupon>)grp.ToImmutableList());
    }
    
    public async Task<IImmutableList<Coupon>> FindCouponsByCustomerId(Guid customerId, CancellationToken cancellationToken = default) {
        return (await Converter.ConvertToDomain(Dao
                .FindBy(StatementParameters.CreateWhere(nameof(Coupon.CustomerId), customerId), cancellationToken), cancellationToken))
            .ToImmutableList();
    }
    public async Task<IImmutableList<Coupon>> FindCouponsByOrderId(Guid orderId, CancellationToken cancellationToken = default) {
        return (await Converter.ConvertToDomain(Dao
                .FindBy(StatementParameters.CreateWhere(nameof(Coupon.OrderId), orderId), cancellationToken), cancellationToken))
            .ToImmutableList();
    }
    public async Task<IImmutableList<Coupon>> FindCouponsByCartId(Guid cartId, CancellationToken cancellationToken = default) {
        return (await Converter.ConvertToDomain(Dao
                .FindBy(StatementParameters.CreateWhere(nameof(Coupon.CartId), cartId), cancellationToken), cancellationToken))
            .ToImmutableList();
    }
    
    public async Task<Coupon> AddAsync(Coupon entity, CancellationToken cancellationToken = default) {
        var dataModel = Converter.ConvertFromDomain(entity);
        await Dao.AddAsync(dataModel, cancellationToken);
        entity = await Converter.ConvertToDomain(dataModel, cancellationToken);
        return entity;
    }
    
    public async Task AddAsync(IEnumerable<Coupon> entities, CancellationToken cancellationToken = default) {
        var dataModels = Converter.ConvertFromDomain(entities);
        await Dao.AddAsync(dataModels, cancellationToken);
    }
    public async Task UpdateAsync(IEnumerable<Coupon> entities, CancellationToken cancellationToken = default) {
        var dataModels = Converter.ConvertFromDomain(entities);
        await Dao.UpdateAsync(dataModels, cancellationToken);
    }
    
    public async Task UpdateCouponsAsync(IEnumerable<Coupon> oldDomainModels, IEnumerable<Coupon> newDomainModels, CancellationToken cancellationToken = default) {
        var oldDataModels = oldDomainModels.Select(Converter.ConvertFromDomain);
        var newDataModels = newDomainModels.Select(Converter.ConvertFromDomain);
        await Dao.ApplyAsync(oldDataModels, newDataModels.ToImmutableArray(), cancellationToken);
    }

    public async Task<Coupon> UpdateAsync(Coupon entity, CancellationToken cancellationToken = default) {
        var dataModel = Converter.ConvertFromDomain(entity);
        dataModel = await Dao.UpdateAsync(dataModel, cancellationToken);
        entity = await Converter.ConvertToDomain(dataModel, cancellationToken);
        return entity;
    }
    public async Task<Coupon> UpdateAsync(Coupon oldEntity, Coupon newEntity, CancellationToken cancellationToken = default) {
        return await UpdateImplAsync(newEntity, cancellationToken);
    }
    
    private async Task<Coupon> UpdateImplAsync(Coupon entity, CancellationToken cancellationToken = default) {
        var dataModel = Converter.ConvertFromDomain(entity);
        dataModel = await Dao.UpdateAsync(dataModel, cancellationToken);
        entity = await Converter.ConvertToDomain(dataModel, cancellationToken);
        return entity;
    }

    public async Task DeleteAsync(Coupon entity, CancellationToken cancellationToken = default) {
        var dataModel = Converter.ConvertFromDomain(entity);
        await Dao.DeleteAsync(dataModel, cancellationToken);
    }
    
    public async Task DeleteAsync(IEnumerable<Coupon> entities, CancellationToken cancellationToken = default) {
        var dataModels = Converter.ConvertFromDomain(entities);
        await Dao.DeleteAsync(dataModels, cancellationToken);
    }
}

internal class CouponDomainModelConvert : IDomainReadModelConverter<CouponDataModel, Coupon> {
    public IEnumerable<OrderParameter>? DefaultOrderParameters { get; } = null;
    
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
    public async ValueTask<Coupon> ConvertToDomain(CouponDataModel dataModel, CancellationToken cancellationToken) {
        return (await ConvertToDomain(new List<CouponDataModel>() { dataModel }, cancellationToken)).First();
    }
    public async Task<IReadOnlyList<Coupon>> ConvertToDomain(IAsyncEnumerable<CouponDataModel> dataModels, CancellationToken cancellationToken = default) {
        var items = await dataModels.ToListAsync(cancellationToken);
        return await ConvertToDomain(items, cancellationToken);
    }
    
    private Task<IReadOnlyList<Coupon>> ConvertToDomain(IReadOnlyCollection<CouponDataModel> dataModels, CancellationToken cancellationToken = default) {
        var domainModels = new List<Coupon>();
        foreach (var dataModel in dataModels) {
            domainModels.Add(new Coupon() {
                Id = dataModel.Id,
                ShopId = dataModel.ShopId,
                Value = dataModel.Value,
                OrderId = dataModel.OrderId,
                CartId = dataModel.CartId,
                CustomerId = dataModel.RedeemedBy,
                ConcurrencyToken = dataModel.GetConcurrencyToken()
            });
        }
        return Task.FromResult<IReadOnlyList<Coupon>>(domainModels);
    }
}