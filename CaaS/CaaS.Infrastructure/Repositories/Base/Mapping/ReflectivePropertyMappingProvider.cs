namespace CaaS.Infrastructure.Repositories.Base.Mapping; 

public class ReflectivePropertyMappingProvider<T> : IPropertyMappingProvider<T> {
    private readonly PropertyNamingPolicy _namingPolicy;
    private readonly IPropertyMapping _cachedMapping;

    public ReflectivePropertyMappingProvider(PropertyNamingPolicy namingPolicy) {
        _namingPolicy = namingPolicy;
        _cachedMapping = GetPropertyMappingImpl();
    }

    public IPropertyMapping GetPropertyMapping() => _cachedMapping;

    private IPropertyMapping GetPropertyMappingImpl() {
        var propertyMapping = new PropertyMapping();
        var properties = typeof(T).GetProperties();
        foreach (var propertyInfo in properties) {
            propertyMapping.AddPropertyMapping(propertyInfo.Name, 
                    _namingPolicy.ConvertName(propertyInfo.Name));
        }
        return propertyMapping;
    }
}