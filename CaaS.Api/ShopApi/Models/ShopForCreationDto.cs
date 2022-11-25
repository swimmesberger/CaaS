namespace CaaS.Api.ShopApi.Models; 

public record ShopForCreationDto(Guid Id, string Name, int CartLifetimeMinutes, Guid ShopAdminId);