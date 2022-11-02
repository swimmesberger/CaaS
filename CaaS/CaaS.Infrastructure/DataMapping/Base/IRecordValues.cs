namespace CaaS.Infrastructure.DataMapping.Base; 

public interface IRecordValues : IPropertyMapper {
    object? GetObject(string key);
}