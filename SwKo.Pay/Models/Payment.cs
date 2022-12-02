using SwKo.Pay.Api;

namespace SwKo.Pay.Models; 

public record Payment {
    public Guid Id { get; init; } = Guid.NewGuid();
    public Customer Customer { get; init; } = null!;
    public decimal Amount { get; set; }
    public Currency Currency { get; set;  }
    public PaymentStatus Status { get; set; }
}