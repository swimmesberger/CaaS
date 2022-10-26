using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace CaaS.Infrastructure.Repositories.Base.Mapping; 

public class ReflectiveRecordMapper<T> : IRecordMapper<T> {
    private readonly PropertyNamingPolicy _namingPolicy;
    private readonly IPropertyMapping _cachedMapping;
    private readonly ObjectActivator _createRecord;
    private readonly IReadOnlyDictionary<string, Func<T, object>> _propAccessors;
    public string MappedTypeName { get; }

    public ReflectiveRecordMapper(PropertyNamingPolicy namingPolicy) {
        _namingPolicy = namingPolicy;
        var properties = typeof(T).GetProperties();
        _cachedMapping = GetPropertyMappingImpl(properties);
        _createRecord = GenerateObjectActivator(properties);
        _propAccessors = GenerateRecordActivators(properties);
        MappedTypeName = _namingPolicy.ConvertName(typeof(T).Name);
    }
    
    private IPropertyMapping GetPropertyMappingImpl(IEnumerable<PropertyInfo> properties) {
        var propertyMapping = new PropertyMapping();
        foreach (var propertyInfo in properties) {
            propertyMapping.AddPropertyMapping(propertyInfo.Name, 
                    _namingPolicy.ConvertName(propertyInfo.Name));
        }
        return propertyMapping;
    }

    public IPropertyMapper ByColumName() => _cachedMapping.ByColumName();

    public IPropertyMapper ByPropertyName() => _cachedMapping.ByPropertyName();

    public T EntityFromRecord(IDataRecord dbRecord) {
        var record = new DatabaseRecord(dbRecord, _cachedMapping);
        return _createRecord.Invoke(record.ByPropertyName());
    }

    public IRecord RecordFromEntity(T value) => new Record(value, _propAccessors, _cachedMapping);

    private delegate T ObjectActivator(IRecordValues record);
    
    // generates lambda to create T with property initializer based on IRecordValues objects
    // record => new Shop() {
    //     Name = Convert(record.GetObject("Name", System.String), String), 
    //     Id = Convert(record.GetObject("Id", System.Guid), Guid), 
    //     RowVersion = Convert(record.GetObject("RowVersion", System.Int32), Int32), 
    //     CreationTime = Convert(record.GetObject("CreationTime", System.DateTimeOffset), DateTimeOffset), 
    //     LastModificationTime = Convert(record.GetObject("LastModificationTime", System.DateTimeOffset), DateTimeOffset)
    // }
    private static ObjectActivator GenerateObjectActivator(IEnumerable<PropertyInfo> properties) {
        var bindings = new List<MemberBinding>();
        var lambdaParam = Expression.Parameter(typeof(IRecordValues), "record");
        var getObjectMethod = typeof(IRecordValues)
                .GetMethod(nameof(IRecordValues.GetObject), types: new[] { typeof(string), typeof(Type) })!;
        foreach (var prop in properties) {
            Expression getObjectExpr = Expression.Call(lambdaParam, getObjectMethod, Expression.Constant(prop.Name), 
                    Expression.Constant(prop.PropertyType));
            getObjectExpr = Expression.Convert(getObjectExpr, prop.PropertyType);
            var assignment = Expression.Bind(prop, getObjectExpr);
            bindings.Add(assignment);
        }
        var ctor = Expression.New(typeof(T));
        var memberInit = Expression.MemberInit(ctor, bindings);
        var lambdaExpr = Expression.Lambda(typeof(ObjectActivator), memberInit, lambdaParam);
        return (ObjectActivator)lambdaExpr.Compile();
    }

    // generate lambda for each property: value => Convert(value.Id, Object)
    private static Dictionary<string, Func<T, object>> GenerateRecordActivators(IEnumerable<PropertyInfo> properties) {
        var lambdaParam = Expression.Parameter(typeof(T), "value");
        var propAccessorDict = new Dictionary<string, Func<T, object>>();
        foreach (var prop in properties) {
            var lambdaExpr = Expression.Lambda(typeof(Func<T, object>), Expression
                    .Convert(Expression.Property(lambdaParam, prop), typeof(object)), lambdaParam);
            propAccessorDict[prop.Name] = (Func<T, object>)lambdaExpr.Compile();
        }
        return propAccessorDict;
    }
    
    private class DatabaseRecord : IRecord {
        private readonly IDataRecord _record;
        private readonly IPropertyMapping _propertyMapping;

        public DatabaseRecord(IDataRecord record, IPropertyMapping propertyMapping) {
            _record = record;
            _propertyMapping = propertyMapping;
        }

        public IRecordValues ByColumName() => new DatabaseRecordValues(_propertyMapping.ByColumName(), _record);

        public IRecordValues ByPropertyName() => new DatabaseRecordValues(_propertyMapping.ByPropertyName(), _record);
    }
    
    private class Record : IRecord {
        private readonly T _value;
        private readonly IReadOnlyDictionary<string, Func<T, object>> _propAccessors;
        private readonly IPropertyMapping _propertyMapping;

        public Record(T value, IReadOnlyDictionary<string, Func<T, object>> propAccessors, IPropertyMapping propertyMapping) {
            _value = value;
            _propAccessors = propAccessors;
            _propertyMapping = propertyMapping;
        }

        public IRecordValues ByColumName() => new RecordValues(_propertyMapping.ByColumName(), _value, _propAccessors);

        public IRecordValues ByPropertyName() => new RecordValues(_propertyMapping.ByPropertyName(), _value, _propAccessors);
    }

    private class RecordValues : AbstractRecordValues {
        private readonly T _value;
        private readonly IReadOnlyDictionary<string, Func<T, object>> _propAccessors;

        public RecordValues(IPropertyMapper propertyMapper, T value, 
                IReadOnlyDictionary<string, Func<T, object>> propAccessors) : base(propertyMapper) {
            _value = value;
            _propAccessors = propAccessors;
        }
        
        public override object? GetObject(string key) {
            return _propAccessors.TryGetValue(MapName(key), out var accessor) ? accessor.Invoke(_value) : null;
        }
    }

    private class DatabaseRecordValues : AbstractRecordValues {
        private readonly IDataRecord _record;

        public DatabaseRecordValues(IPropertyMapper propertyMapper, IDataRecord record) : base(propertyMapper) {
            _record = record;
        }
        
        public override object? GetObject(string key) {
            try {
                return _record[MapName(key)];
            } catch (IndexOutOfRangeException) {
                return null;
            }
        }
    }
}