using CaaS.Core.Entities.Base;

namespace CaaS.Core.Entities; 

public record Product : IEntityBase {
    public Guid Id { get; init; }
    public Shop Shop { get; init; } = null!;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string DownloadLink { get; init; } = string.Empty;
    public decimal Price { get; init; }

    public string ConcurrencyToken { get; init; } = string.Empty;
}