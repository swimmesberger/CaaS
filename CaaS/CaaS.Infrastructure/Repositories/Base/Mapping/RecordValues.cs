using System.Data;

namespace CaaS.Infrastructure.Repositories.Base.Mapping; 

public class RecordValues {
    private readonly IDataRecord _record;
    private readonly IPropertyMapping _propertyMapping;

    public RecordValues(IDataRecord record, IPropertyMapping propertyMapping) {
        _record = record;
        _propertyMapping = propertyMapping;
    }
    
    public DateTimeOffset GetDateTimeOffset(string propertyName) {
        var obj = _record[_propertyMapping.MapProperty(propertyName)];
        if (obj is DateTimeOffset dateTimeOffset) return dateTimeOffset;
        if (obj is DateTime dateTime) return new DateTimeOffset(dateTime);
        throw new InvalidCastException();
    }
    
    public int GetIn32(string propertyName) {
        return (int)_record[_propertyMapping.MapProperty(propertyName)];
    }
    
    public Guid GetGuid(string propertyName) {
        return (Guid)_record[_propertyMapping.MapProperty(propertyName)];
    }
    
    public string GetString(string propertyName) {
        return (string)_record[_propertyMapping.MapProperty(propertyName)];
    }
}