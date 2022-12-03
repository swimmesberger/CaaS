using CaaS.Infrastructure.Base.Ado.Model;
using CaaS.Infrastructure.Base.Mapping;

namespace CaaS.Infrastructure.Base.Ado; 

public static class RecordValuesExtensions {
    public static TypedValue GetTypedValue(this IRecordValues recordValues, string key) {
        var objectType = recordValues.GetObjectType(key);
        if (objectType == IRecordValues.DbTypeJson) {
            return TypedValue.CreateJsonValue(recordValues.GetObject(key));
        }
        return new TypedValue(recordValues.GetObject(key));
    }
}