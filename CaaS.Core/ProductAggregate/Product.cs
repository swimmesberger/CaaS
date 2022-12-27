using CaaS.Core.Base;
using CaaS.Core.ShopAggregate;

namespace CaaS.Core.ProductAggregate; 

public record Product : IEntityBase {
    public Guid Id { get; init; } = Guid.NewGuid();
    public Shop Shop { get; init; } = null!;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string DownloadLink { get; init; } = string.Empty;
    public string ImageSrc { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public bool Deleted { get; init; } = false;
    public string ConcurrencyToken { get; init; } = string.Empty;
}