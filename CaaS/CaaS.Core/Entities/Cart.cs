using CaaS.Core.Entities.Base;

namespace CaaS.Core.Entities; 

public class Cart : IEntityBase {
    public Guid Id { get; init; }
    public Shop Shop { get; init; } = null!;
    public List<Product> Products { get; init; } = new List<Product>();

    public string ConcurrencyToken { get; init; } = string.Empty;
}