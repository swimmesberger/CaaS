namespace CaaS.Core.DiscountAggregate.Models;

public record DiscountParameters {
    public static readonly DiscountParameters Empty = new DiscountParameters();
            
    public int Version { get; init; } = 1;
    public string Name { get; init; } = string.Empty;

    public DiscountParameters(int version, string? name = null) {
        Version = version;
        Name = name ?? string.Empty;
    }

    public DiscountParameters() { }
}