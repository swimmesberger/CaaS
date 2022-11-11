using CaaS.Infrastructure.Base.Mapping;
using CaaS.Infrastructure.Base.Model;

namespace CaaS.Infrastructure.ShopData; 

[GenerateMapper]
public record ShopAdminDataModel : DataModel {
    // [TenantIdColumn] - not usable because referenced by Shop
    public Guid ShopId { get; init; } = default;
    public string Name { get; init; } = string.Empty;
    public string EMail { get; init; } = string.Empty;
}