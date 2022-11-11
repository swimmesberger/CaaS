// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace CaaS.Infrastructure.Base.Mapping; 

public class GenerateMapper : Attribute {
    public Type[] EntityTypes { get; }
    public PropertyMappingPolicy NamingPolicyType { get; }

    public GenerateMapper() : this((Type?)null) { }

    public GenerateMapper(Type? entityType, PropertyMappingPolicy namingPolicyType = PropertyMappingPolicy.Undefined) : 
            this(entityType == null ? Type.EmptyTypes : new[] { entityType }, namingPolicyType) { }

    public GenerateMapper(Type[] entityTypes, PropertyMappingPolicy namingPolicyType = PropertyMappingPolicy.Undefined) {
        EntityTypes = entityTypes;
        NamingPolicyType = namingPolicyType;
    }
}