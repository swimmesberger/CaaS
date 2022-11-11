namespace CaaS.Infrastructure.Base.Mapping; 

public interface IPropertyMapper {
    IEnumerable<string> Keys { get; }

    string MapName(string key);
}