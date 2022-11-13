namespace CaaS.Api.Base.Attributes;

public class ProducesHeaderAttribute : Attribute {
    public string Name { get; }
    public string? Description { get; }
    public Type? Type { get; }

    public ProducesHeaderAttribute(string name, Type? type = null, string? description = null) {
        Name = name;
        Type = type;
        Description = description;
    }
}