using CaaS.Core.Base;

namespace CaaS.Core.CustomerAggregate;

public record Customer : IEntityBase {
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid ShopId { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string EMail { get; init; } = string.Empty;
    public string TelephoneNumber { get; init; } = string.Empty;
    public string CreditCardNumber { get; init; } = string.Empty;
    public string ConcurrencyToken { get; init; } = string.Empty;
}