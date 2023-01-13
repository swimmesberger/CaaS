// ReSharper disable NotAccessedPositionalProperty.Global

using System.ComponentModel.DataAnnotations;

namespace CaaS.Api.ProductApi.Models; 

public record ProductForCreationDto(
    Guid Id, 
    string Name, 
    string Description, 
    [Required] decimal? Price,
    string DownloadLink,
    string ImageSrc
);