using System.ComponentModel.DataAnnotations;

namespace CaaS.Api.ShopApi.Models; 

public record ShopForUpdateDto(
    string Name, 
    [Required] int? CartLifetimeMinutes, 
    ShopAdminForUpdateDto ShopAdmin, 
    string AppKey, 
    string ConcurrencyToken
);

public record ShopAdminForUpdateDto(
    Guid? Id, 
    string Name, 
    string EMail
);