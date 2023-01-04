// ReSharper disable NotAccessedPositionalProperty.Global
namespace CaaS.Api.CustomerApi.Models; 

public record CustomerForCreationDto(Guid Id, string FirstName, string LastName, string EMail, string TelephoneNumber, string CreditCardNumber);