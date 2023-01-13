using CaaS.Api.BlobApi;
using CaaS.Core.Base.Url;

namespace CaaS.Api.Base; 

public sealed class WebBlobLinkGenerator : ILinkGenerator {
    private readonly HttpContext? _httpContext;
    private readonly LinkGenerator _linkGenerator;
    
    public WebBlobLinkGenerator(IHttpContextAccessor httpContextAccessor, LinkGenerator linkGenerator) {
        _httpContext = httpContextAccessor.HttpContext;
        _linkGenerator = linkGenerator;
    }

    public string CreateAbsoluteUrl(string relativeUrl) {
        if (string.IsNullOrEmpty(relativeUrl) || _httpContext == null) return relativeUrl;
        return _linkGenerator.GetUriByAction(
            httpContext: _httpContext,
            controller: "Blob",
            action: nameof(BlobController.Get),
            values: new { path = relativeUrl }
        ) ?? throw new InvalidOperationException();
    }
    
    public string CreateRelativeUrl(string absoluteUrl) {
        return absoluteUrl;
    }
}