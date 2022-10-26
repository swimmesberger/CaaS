namespace CaaS.Infrastructure.Repositories.Base.Mapping;

public interface IPropertyMapping {
    IPropertyMapper ByColumName();

    IPropertyMapper ByPropertyName();
}

public interface IPropertyMapper {
    IEnumerable<string> Keys { get; }
    string MapName(string key);
}