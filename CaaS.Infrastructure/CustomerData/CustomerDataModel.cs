using CaaS.Infrastructure.Base.Mapping;
using CaaS.Infrastructure.Base.Model;

namespace CaaS.Infrastructure.CustomerData;

[GenerateMapper]
public record CustomerDataModel : DataModel {
    [TenantIdColumn]
    public Guid ShopId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string EMail { get; init; } = string.Empty;
    public string CreditCardNumber { get; init; } = string.Empty;
}