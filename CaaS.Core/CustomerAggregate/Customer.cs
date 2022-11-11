using CaaS.Core.Base;

namespace CaaS.Core.CustomerAggregate;

public record Customer : IEntityBase {
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; init; } = string.Empty;
    public Guid ShopId { get; init; }
    public string EMail { get; init; } = string.Empty;
    public string CreditCardNumber { get; init; } = string.Empty;
    public string ConcurrencyToken { get; init; } = string.Empty;
}