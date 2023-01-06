using System.Net;
using System.Net.Mime;
using CaaS.Api.Base.AppKey;
using CaaS.Api.Base.Attributes;
using CaaS.Core.BlobAggregate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace CaaS.Api.BlobApi; 

[RequireTenant]
[ApiController]
[Route("Blob/{**path}")]
public class BlobController : ControllerBase {
    private readonly IBlobService _blobService;

    public BlobController(IBlobService blobService) {
        _blobService = blobService;
    }

    [Produces(MediaTypeNames.Application.Octet, Type = typeof(FileResult))]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [HttpGet]
    public async Task<IActionResult> Get([FromRoute] string path, CancellationToken cancellationToken = default) {
        path = WebUtility.UrlDecode(path);
        var blobItem = await _blobService.GetAsync(path, cancellationToken);
        if (blobItem == null) {
            return NotFound();
        }
        // treat empty blobs as not found because we sometimes create empty blobs before a file is uploaded
        if (blobItem.Blob.Length <= 0) {
            return NotFound();
        }
        var lastModTime = blobItem.LastModificationTime;
        var etagValue =
            $"\"{lastModTime.Year}{lastModTime.Month}{lastModTime.Day}{lastModTime.Hour}{lastModTime.Minute}{lastModTime.Second}{lastModTime.Millisecond}\"";
        var etag = new EntityTagHeaderValue(etagValue);
        var mimeType = blobItem.MimeType;
        if (string.IsNullOrEmpty(mimeType)) {
            mimeType = MediaTypeNames.Application.Octet;
        }
        // TODO: ToArray can be replaced in .NET 7
        return File(blobItem.Blob.ToArray(), mimeType, blobItem.LastModificationTime, etag);
    }
    
    [Authorize(Policy = AppKeyAuthenticationDefaults.AuthorizationPolicy)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpPost]
    public async Task AddOrUpdate([FromRoute] string path, IFormFile file, CancellationToken cancellationToken = default) {
        var memory = new byte[file.Length];
        using var memoryStream = new MemoryStream(memory);
        await file.CopyToAsync(memoryStream, cancellationToken);
        await _blobService.AddOrUpdateAsync(new BlobItem() {
            MimeType = file.ContentType,
            Name = SanitizeForFileName(file.FileName),
            Path = path,
            Blob = memory
        }, cancellationToken);
    }
    
    private static string SanitizeForFileName(string s) {
        return Path.GetInvalidFileNameChars()
            .Aggregate(s, (current, c) => current.Replace(c, '_'))
            .ToLowerInvariant();
    }
}