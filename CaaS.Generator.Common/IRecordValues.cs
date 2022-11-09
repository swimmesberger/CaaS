namespace CaaS.Generator.Common {
    public interface IRecordValues : IPropertyMapper {
        object? GetObject(string key);
    }
}