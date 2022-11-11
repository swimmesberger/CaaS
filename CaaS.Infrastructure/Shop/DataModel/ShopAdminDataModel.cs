using CaaS.Infrastructure.Base.DataMapping;

namespace CaaS.Infrastructure.Shop.DataModel; 

[GenerateMapper]
public record ShopAdminDataModel : Base.DataModel.DataModel {
    // [TenantIdColumn] - not usable because referenced by Shop
    public Guid ShopId { get; init; } = default;
    public string Name { get; init; } = string.Empty;
    public string EMail { get; init; } = string.Empty;
}