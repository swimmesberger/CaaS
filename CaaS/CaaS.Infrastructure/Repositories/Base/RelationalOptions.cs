namespace CaaS.Infrastructure.Repositories.Base;

public record RelationalOptions {
    public const string Key = "ConnectionStrings:Main";
    
    public string ConnectionString { get; init; } = string.Empty;
    public string ProviderName { get; init; } = string.Empty;
    
    public RelationalOptions() { }

    public RelationalOptions(string connectionString, string providerName) {
        ConnectionString = connectionString;
        ProviderName = providerName;
    }
}