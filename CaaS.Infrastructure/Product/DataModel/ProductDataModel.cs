using CaaS.Infrastructure.Base.DataMapping;

namespace CaaS.Infrastructure.Product.DataModel; 

[GenerateMapper]
public record ProductDataModel : Base.DataModel.DataModel {
    [TenantIdColumn]
    public Guid ShopId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string DownloadLink { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public bool Deleted { get; init; }
}