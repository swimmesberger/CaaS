using CaaS.Infrastructure.Base.Mapping;
using CaaS.Infrastructure.Base.Model;

namespace CaaS.Infrastructure.ProductData; 

[GenerateMapper]
public record ProductDataModel : DataModel {
    [TenantIdColumn]
    public Guid ShopId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string DownloadLink { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public bool Deleted { get; init; }
}