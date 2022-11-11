using CaaS.Infrastructure.Base.DataMapping;

namespace CaaS.Infrastructure.Base.Ado.Model; 

public static class RecordValuesExtensions {
    public static TypedValue GetTypedValue(this IRecordValues recordValues, string key) {
        var objectType = recordValues.GetObjectType(key);
        if (objectType == IRecordValues.DbTypeJson) {
            return TypedValue.CreateJsonValue(recordValues.GetObject(key));
        }
        return new TypedValue(recordValues.GetObject(key));
    }
}