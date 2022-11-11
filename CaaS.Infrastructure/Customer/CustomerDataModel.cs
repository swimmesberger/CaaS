using CaaS.Infrastructure.Base.DataMapping;
using CaaS.Infrastructure.Base.DataModel;

namespace CaaS.Infrastructure.Customer;

[GenerateMapper]
public record CustomerDataModel : DataModel {
    [TenantIdColumn]
    public Guid ShopId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string EMail { get; init; } = string.Empty;
    public string CreditCardNumber { get; init; } = string.Empty;
}