namespace CaaS.Core.Tenant;

public record Tenant(string Id, string Name) {
    public static readonly Tenant Empty = new Tenant(string.Empty, string.Empty);
}