using System.Collections.Specialized;

namespace CaaS.Infrastructure.Repositories.Base.Mapping;

public class PropertyMapping : IPropertyMapping {
    // OrderedDictionary -> keep insertion order
    private readonly PropertyMapper _propertyToColumnMapping;
    private readonly PropertyMapper _columnToPropertyMapping;

    public PropertyMapping() {
        _propertyToColumnMapping = new PropertyMapper();
        _columnToPropertyMapping = new PropertyMapper();
    }

    public void AddPropertyMapping(string propertyName, string columnName) {
        _propertyToColumnMapping[propertyName] = columnName;
        _columnToPropertyMapping[columnName] = propertyName;
    }
    
    public void AddColumnMapping(string columnName, string propertyName) {
        _propertyToColumnMapping[propertyName] = columnName;
        _columnToPropertyMapping[columnName] = propertyName;
    }

    public IPropertyMapper ByColumName() => _columnToPropertyMapping;
    public IPropertyMapper ByPropertyName() => _propertyToColumnMapping;
}

public class PropertyMapper : IPropertyMapper {
    private readonly OrderedDictionary _mapping;
    public IEnumerable<string> Keys => _mapping.Keys.Cast<string>();
    
    internal string this[string key] {
        get => (string)_mapping[key]!;
        set => _mapping[key] = value;
    }

    public PropertyMapper(OrderedDictionary mapping) {
        _mapping = mapping;
    }

    public PropertyMapper(): this(new OrderedDictionary()) { }

    public string MapName(string key) => (string)_mapping[key]!;
}