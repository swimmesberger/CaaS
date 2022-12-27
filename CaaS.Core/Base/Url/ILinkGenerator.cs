namespace CaaS.Core.Base.Url; 

public interface ILinkGenerator {
    string CreateAbsoluteUrl(string relativeUrl);
    
    string CreateRelativeUrl(string absoluteUrl);
}

public sealed class NoOpLinkGenerator : ILinkGenerator {
    public static readonly NoOpLinkGenerator Instance = new NoOpLinkGenerator();
    
    public string CreateAbsoluteUrl(string relativeUrl) => relativeUrl;
    public string CreateRelativeUrl(string absoluteUrl) => absoluteUrl;
}