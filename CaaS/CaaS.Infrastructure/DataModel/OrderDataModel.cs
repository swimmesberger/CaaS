using CaaS.Infrastructure.DataMapping;

namespace CaaS.Infrastructure.DataModel;

[GenerateMapper]
public record OrderDataModel : Base.DataModel {
    [TenantIdColumn]
    public Guid ShopId { get; init; }
    public int OrderNumber { get; init; }
    public Guid CustomerId { get; init; }
    public DateTimeOffset OrderDate { get; init; }
}