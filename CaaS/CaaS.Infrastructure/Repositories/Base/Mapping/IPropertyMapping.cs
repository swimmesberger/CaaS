namespace CaaS.Infrastructure.Repositories.Base.Mapping;

public interface IPropertyMapping {
    IEnumerable<string> Properties { get; }
    IEnumerable<string> Columns { get; }
    
    string MapColumn(string columnName);

    string MapProperty(string propertyName);
}