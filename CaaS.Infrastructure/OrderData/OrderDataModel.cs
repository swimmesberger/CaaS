using CaaS.Infrastructure.Base.Mapping;
using CaaS.Infrastructure.Base.Model;

namespace CaaS.Infrastructure.OrderData;

[GenerateMapper]
public record OrderDataModel : DataModel {
    [TenantIdColumn]
    public Guid ShopId { get; init; }
    public int OrderNumber { get; init; }
    public Guid CustomerId { get; init; }
    public DateTimeOffset OrderDate { get; init; } = DateTimeOffset.Now;
}