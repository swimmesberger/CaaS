using System.Data;

namespace CaaS.Infrastructure.Repositories.Base.Mapping; 

public static class IDataRecordExtensions {
    public static DateTimeOffset GetDateTimeOffset(this IDataRecord dataRecord, string fieldName) {
        var obj = dataRecord[fieldName];
        if (obj is DateTimeOffset dateTimeOffset) return dateTimeOffset;
        if (obj is DateTime dateTime) return new DateTimeOffset(dateTime);
        throw new InvalidCastException();
    }
    
    public static int GetIn32(this IDataRecord dataRecord, string fieldName) {
        return (int)dataRecord[fieldName];
    }
    
    public static Guid GetGuid(this IDataRecord dataRecord, string fieldName) {
        return (Guid)dataRecord[fieldName];
    }
    
    public static string GetString(this IDataRecord dataRecord, string fieldName) {
        return (string)dataRecord[fieldName];
    }
}