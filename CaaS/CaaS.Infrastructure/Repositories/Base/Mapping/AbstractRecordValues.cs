namespace CaaS.Infrastructure.Repositories.Base.Mapping; 

public abstract class AbstractRecordValues : IRecordValues {
    private readonly IPropertyMapper _propertyMapper;

    public IEnumerable<string> Keys => _propertyMapper.Keys;

    protected AbstractRecordValues(IPropertyMapper propertyMapper) {
        _propertyMapper = propertyMapper;
    }

    public string MapName(string key) => _propertyMapper.MapName(key);

    public abstract object? GetObject(string key); 
    
    public object? GetObject(string key, Type targetType) {
        if (targetType == typeof(DateTimeOffset)) {
            return GetDateTimeOffset(key);
        }
        if (targetType == typeof(int)) {
            return GetIn32(key);
        }
        if (targetType == typeof(Guid)) {
            return GetGuid(key);
        }
        if (targetType == typeof(string)) {
            return GetString(key);
        }
        return GetObject(key);
    }

    public DateTimeOffset? GetDateTimeOffset(string key) {
        var obj = GetObject(key);
        if (obj == null) return null;
        if (obj is DateTimeOffset dateTimeOffset) return dateTimeOffset;
        if (obj is DateTime dateTime) return new DateTimeOffset(dateTime);
        throw new InvalidCastException();
    }
        
    public int? GetIn32(string key) {
        var obj = GetObject(key);
        return (int?)obj;
    }
        
    public Guid? GetGuid(string key) {
        var obj = GetObject(key);
        return (Guid?)obj;
    }
        
    public string? GetString(string key) {
        var obj = GetObject(key);
        return (string?)obj;
    }
}