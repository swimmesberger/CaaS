using CaaS.Core.Base;
using CaaS.Core.Base.Pagination;

namespace CaaS.Core.CouponAggregate; 

public interface ICouponRepository : IRepository {
    Task<Coupon?> FindByIdAsync(Guid couponId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Coupon>> FindByAsync(CouponQuery couponQuery, CancellationToken cancellationToken = default);
    Task<PagedResult<Coupon>> FindByPagedAsync(CouponQuery couponQuery, PaginationToken? paginationToken = null,
        CancellationToken cancellationToken = default);

    Task<Dictionary<Guid, IReadOnlyList<Coupon>>> FindByOrderIdsAsync(IReadOnlyCollection<Guid> orderIds, CancellationToken cancellationToken = default);
    Task<Dictionary<Guid, IReadOnlyList<Coupon>>> FindByCartIdsAsync(IReadOnlyCollection<Guid> cartIds, CancellationToken cancellationToken = default);

    Task<Coupon> AddAsync(Coupon entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(IEnumerable<Coupon> oldDomainModels, IEnumerable<Coupon> newDomainModels,
        CancellationToken cancellationToken = default);
    Task<Coupon> UpdateAsync(Coupon oldEntity, Coupon newEntity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Coupon coupon, CancellationToken cancellationToken = default);

    Task<long> CountAsync(CancellationToken cancellationToken = default);
}