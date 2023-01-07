using CaaS.Infrastructure.Base.Mapping;
using CaaS.Infrastructure.Base.Model;

namespace CaaS.Infrastructure.CouponData;

[GenerateMapper]
public record CouponDataModel : DataModel {
    [TenantIdColumn]
    public Guid ShopId { get; init; }
    public string Code { get; init; } = string.Empty;
    public decimal Value { get; init; }
    public Guid? OrderId { get; init; }
    public Guid? CartId { get; init; }
    public Guid? CustomerId { get; init; }
}