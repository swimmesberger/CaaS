namespace SwKo.Pay.Models; 

public record CreditCard {
    public String CreditCardNumber { get; init; } = String.Empty;
    public bool Active { get; init; } = true;
    public decimal Credit { get; init; } = 0;
}