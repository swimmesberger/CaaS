namespace CaaS.Infrastructure.Repositories.Base.Mapping;

// ReSharper disable once UnusedTypeParameter
public interface IPropertyMappingProvider<T> {
    IPropertyMapping GetPropertyMapping();
}