using System.ComponentModel.DataAnnotations;

namespace CaaS.Api.ProductApi.Models; 

public record ProductForUpdateDto(Guid Id, [Required] string? Name, [Required] string? Description, 
                                    [Required] string? DownloadLink, [Required] decimal? Price, string ConcurrencyToken);