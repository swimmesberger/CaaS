using CaaS.Core.Base;
using CaaS.Infrastructure.Base.Mapping;
using CaaS.Infrastructure.Base.Model;

namespace CaaS.Infrastructure.OrderData;

[GenerateMapper]
public record OrderDataModel : DataModel {
    [TenantIdColumn]
    public Guid ShopId { get; init; }
    public int OrderNumber { get; init; }
    public Guid CustomerId { get; init; }
    public string AddressStreet { get; init; } = string.Empty;
    public string AddressCity { get; init; } = string.Empty;
    public string AddressState { get; init; } = string.Empty;
    public string AddressCountry { get; init; } = string.Empty;
    public string AddressZipCode { get; init; } = string.Empty;
    public DateTimeOffset OrderDate { get; init; } = DateTimeOffsetProvider.GetNow();
}