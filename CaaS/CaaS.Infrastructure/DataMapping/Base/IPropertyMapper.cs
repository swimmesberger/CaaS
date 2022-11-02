namespace CaaS.Infrastructure.DataMapping.Base; 

public interface IPropertyMapper {
    IEnumerable<string> Keys { get; }

    string MapName(string key);
}