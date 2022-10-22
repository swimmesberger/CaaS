namespace CaaS.Infrastructure.Repositories.Base.Mapping;

public abstract class PropertyNamingPolicy {
    protected PropertyNamingPolicy() { }
    
    public static PropertyNamingPolicy SnakeCase { get; } = new PropertySnakeCaseNamingPolicy();

    public abstract string ConvertName(string name);
}