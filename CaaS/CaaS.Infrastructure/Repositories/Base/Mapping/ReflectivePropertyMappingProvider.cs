﻿namespace CaaS.Infrastructure.Repositories.Base.Mapping; 

public class ReflectivePropertyMappingProvider<T> : IPropertyMappingProvider {
    public static readonly IPropertyMappingProvider SnakeCaseInstance = new ReflectivePropertyMappingProvider<T>();
    
    private readonly PropertyNamingPolicy _namingPolicy;
    private readonly IPropertyMapping _cachedMapping;

    private ReflectivePropertyMappingProvider(PropertyNamingPolicy? namingPolicy = null) {
        // map to snake_case on default
        _namingPolicy = namingPolicy ?? PropertyNamingPolicy.SnakeCase;
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