namespace CaaS.Api.CustomerApi.Models; 

public record CustomerDto {
    public static readonly CustomerDto Empty = new CustomerDto();
    
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public Guid ShopId { get; init; }
    public string EMail { get; init; } = string.Empty;
}