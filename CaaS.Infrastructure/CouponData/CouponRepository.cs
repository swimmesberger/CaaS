using System.Collections.Immutable;
using CaaS.Core.CouponAggregate;
using CaaS.Infrastructure.Base.Ado;
using CaaS.Infrastructure.Base.Ado.Query.Parameters;
using CaaS.Infrastructure.Base.Repository;

namespace CaaS.Infrastructure.CouponData; 

public class CouponRepository : CrudRepository<CouponDataModel, Coupon>, ICouponRepository {
    internal new CouponDomainModelConvert Converter => (CouponDomainModelConvert)base.Converter;

    public CouponRepository(IDao<CouponDataModel> dao): 
                            base(dao, new CouponDomainModelConvert()) { }

    public async Task<Dictionary<Guid, IReadOnlyList<Coupon>>> FindByOrderIdsAsync(IReadOnlyCollection<Guid> orderIds,
        CancellationToken cancellationToken = default) {
        return (await Converter
                .ConvertToDomain(Dao
                    .FindBy(StatementParameters.CreateWhere(nameof(CouponDataModel.OrderId), orderIds), cancellationToken), cancellationToken))
            .GroupBy(i => i.OrderId!.Value) //orderId cant be null because of where statement
            .ToDictionary(grp => grp.Key, grp => (IReadOnlyList<Coupon>)grp.ToList());
    }
    
    public async Task<Dictionary<Guid, IReadOnlyList<Coupon>>> FindByCartIdsAsync(IReadOnlyCollection<Guid> cartIds,
        CancellationToken cancellationToken = default) {

        if (cartIds.Count.Equals(0)) {
            return new Dictionary<Guid, IReadOnlyList<Coupon>>();
        }
        
        return (await Converter
                .ConvertToDomain(Dao
                    .FindBy(StatementParameters.CreateWhere(nameof(CouponDataModel.CartId), cartIds), cancellationToken), cancellationToken))
            .GroupBy(i => i.CartId!.Value) //cartId cant be null because of where statement
            .ToDictionary(grp => grp.Key, grp => (IReadOnlyList<Coupon>)grp.ToList());
    }

    public async Task<IReadOnlyList<Coupon>> FindByAsync(CouponQuery couponQuery, CancellationToken cancellationToken = default) {
        var whereParameters = new List<QueryParameter>();
        if (couponQuery.CartId != null) {
            whereParameters.Add(new QueryParameter(nameof(CouponDataModel.CartId), couponQuery.CartId));
        }
        if (couponQuery.OrderId != null) {
            whereParameters.Add(new QueryParameter(nameof(CouponDataModel.OrderId), couponQuery.OrderId));
        }
        if (couponQuery.CustomerId != null) {
            whereParameters.Add(new QueryParameter(nameof(CouponDataModel.CustomerId), couponQuery.CustomerId));
        }
        if (couponQuery.Code != null) {
            whereParameters.Add(new QueryParameter(nameof(CouponDataModel.Code), couponQuery.Code));
        }
        return (await Converter.ConvertToDomain(Dao
                .FindBy(new StatementParameters(){ Where = whereParameters }, cancellationToken), cancellationToken))
            .ToList();
    }

    public async Task UpdateAsync(IEnumerable<Coupon> oldDomainModels, IEnumerable<Coupon> newDomainModels, CancellationToken cancellationToken = default) {
        var oldDataModels = oldDomainModels.Select(Converter.ConvertFromDomain);
        var newDataModels = newDomainModels.Select(Converter.ConvertFromDomain);
        await Dao.ApplyAsync(oldDataModels, newDataModels.ToList(), cancellationToken);
    }
}

internal class CouponDomainModelConvert : IDomainModelConverter<CouponDataModel, Coupon> {
    public CouponDataModel ApplyDomainModel(CouponDataModel dataModel, Coupon domainModel) {
        return dataModel with {
            ShopId = domainModel.ShopId,
            Value = domainModel.Value,
            OrderId = domainModel.OrderId,
            CartId = domainModel.CartId,
            CustomerId = domainModel.CustomerId
        };
    }
    
    public Coupon ApplyDataModel(Coupon domainModel, CouponDataModel dataModel) {
        return domainModel with { ConcurrencyToken = dataModel.GetConcurrencyToken() };
    }
    
    public IReadOnlyList<CouponDataModel> ConvertFromDomain(IEnumerable<Coupon> domainModels)
        => domainModels.Select(ConvertFromDomain).ToImmutableArray();

    public CouponDataModel ConvertFromDomain(Coupon domainModel) {
        return new CouponDataModel {
            Id = domainModel.Id,
            ShopId = domainModel.ShopId,
            Code = domainModel.Code,
            Value = domainModel.Value,
            OrderId = domainModel.OrderId,
            CartId = domainModel.CartId,
            CustomerId = domainModel.CustomerId,
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
                Code = dataModel.Code,
                Value = dataModel.Value,
                OrderId = dataModel.OrderId,
                CartId = dataModel.CartId,
                CustomerId = dataModel.CustomerId,
                ConcurrencyToken = dataModel.GetConcurrencyToken()
            })
            .ToList();
    }
}