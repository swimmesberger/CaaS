using CaaS.Core.Base;
using CaaS.Infrastructure.Base.Mapping;
using CaaS.Infrastructure.Base.Model;

namespace CaaS.Infrastructure.CartData;

[GenerateMapper]
public record CartDataModel : DataModel {
    [TenantIdColumn]
    public Guid ShopId { get; init; }
    public Guid? CustomerId { get; init; }
    public DateTimeOffset LastAccess { get; init; } = DateTimeOffsetProvider.GetNow();
}