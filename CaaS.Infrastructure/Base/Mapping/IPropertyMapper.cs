namespace CaaS.Infrastructure.Base.Mapping; 

public interface IPropertyMapper {
    string TypeName { get; }
    string MappedTypeName { get; }
    
    IEnumerable<string> Keys { get; }

    string MapName(string key);
}