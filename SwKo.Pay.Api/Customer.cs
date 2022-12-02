namespace SwKo.Pay.Api; 

public record Customer {
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Email { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string CreditCardNumber { get; init; } = string.Empty;
    public Dictionary<string, string> Metadata { get; init; } = new();
}