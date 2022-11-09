namespace CaaS.Api; 

public static class HeaderConstants {
    public const string TotalCount = "X-total-count";
    public const string TotalCountDescription = "Number of total entries";
    public const string TenantId = "X-tenant-id";
    public const string TenantIdDescription = "The id of the tenant this operation should be executed on";
    public const string AppKey = "X-caas-key";
}