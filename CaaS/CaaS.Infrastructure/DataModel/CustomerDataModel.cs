using System.Numerics;
using CaaS.Infrastructure.DataMapping;

namespace CaaS.Infrastructure.DataModel;

[GenerateMapper]
public record CustomerDataModel() : Base.DataModel {
    [TenantIdColumn]
    public Guid ShopId { get; init; }
    public string Name { get; init; } = String.Empty;
    public string EMail { get; init; } = String.Empty;
    public string CreditCartNumber { get; init; } = String.Empty;
}