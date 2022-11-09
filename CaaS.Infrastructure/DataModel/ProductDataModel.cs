using CaaS.Infrastructure.DataMapping;

namespace CaaS.Infrastructure.DataModel; 

[GenerateMapper]
public record ProductDataModel : Base.DataModel {
    [TenantIdColumn]
    public Guid ShopId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string DownloadLink { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public bool Deleted { get; init; }
}