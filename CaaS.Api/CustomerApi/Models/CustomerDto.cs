namespace CaaS.Api.CustomerApi.Models; 

public record CustomerDto {
    public static readonly CustomerDto Empty = new CustomerDto();
    
    public Guid Id { get; init; }
    public Guid ShopId { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string EMail { get; init; } = string.Empty;
    public string TelephoneNumber { get; init; } = string.Empty;
    public string ConcurrencyToken { get; init; } = string.Empty;
}