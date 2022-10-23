namespace CaaS.Api.Swagger; 

public class ConsumesHeaderAttribute : Attribute {
    public string Name { get; }
    public string? Description { get; }
    public Type? Type { get; }
    public bool IsRequired { get; }
    
    public ConsumesHeaderAttribute(string name, string? description = null, Type? type = null, bool isRequired = false) {
        Name = name;
        Type = type;
        Description = description;
        IsRequired = isRequired;
        Type = type;
    }
}