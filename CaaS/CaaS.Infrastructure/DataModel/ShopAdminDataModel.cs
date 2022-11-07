using CaaS.Infrastructure.DataMapping;

namespace CaaS.Infrastructure.DataModel; 

[GenerateMapper]
public record ShopAdminDataModel : Base.DataModel {
    // [TenantIdColumn] - not usable because referenced by Shop
    public Guid ShopId { get; init; } = default;
    public string Name { get; init; } = string.Empty;
    public string EMail { get; init; } = string.Empty;
}