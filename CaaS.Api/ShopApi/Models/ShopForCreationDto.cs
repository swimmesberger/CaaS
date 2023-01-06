// ReSharper disable NotAccessedPositionalProperty.Global
namespace CaaS.Api.ShopApi.Models; 

public record ShopForCreationDto(
    Guid Id, 
    string Name, 
    int CartLifetimeMinutes, 
    ShopAdminForCreationDto ShopAdmin, 
    string AppKey
);

public record ShopAdminForCreationDto(
    Guid Id, 
    string Name, 
    string EMail
);