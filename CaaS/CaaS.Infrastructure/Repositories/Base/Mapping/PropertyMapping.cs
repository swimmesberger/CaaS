using System.Collections.Specialized;

namespace CaaS.Infrastructure.Repositories.Base.Mapping;

public class PropertyMapping : IPropertyMapping {
    // OrderedDictionary -> keep insertion order
    private readonly OrderedDictionary _propertyToColumnMapping;
    private readonly OrderedDictionary _columnToPropertyMapping;

    public IEnumerable<string> Properties => _propertyToColumnMapping.Keys.Cast<string>();
    public IEnumerable<string> Columns => _columnToPropertyMapping.Keys.Cast<string>();

    public PropertyMapping() {
        _propertyToColumnMapping = new OrderedDictionary();
        _columnToPropertyMapping = new OrderedDictionary();
    }

    public void AddPropertyMapping(string propertyName, string columnName) {
        _propertyToColumnMapping[propertyName] = columnName;
        _columnToPropertyMapping[columnName] = propertyName;
    }
    
    public void AddColumnMapping(string columnName, string propertyName) {
        _propertyToColumnMapping[propertyName] = columnName;
        _columnToPropertyMapping[columnName] = propertyName;
    }

    public string MapColumn(string columnName) => (string)_columnToPropertyMapping[columnName]!;

    public string MapProperty(string propertyName) => (string)_propertyToColumnMapping[propertyName]!;
}