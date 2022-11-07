using System.Data;
using System.Data.Common;
using CaaS.Infrastructure.Ado.Model;
using Npgsql;
using NpgsqlTypes;

namespace CaaS.Infrastructure.Ado; 

public static class DbCommandExtensions {
    public static DbParameter SetParameter(this DbParameter parameter, string name, TypedValue typedValue = default) {
        parameter.ParameterName = name;
        if (typedValue.IsJson && parameter is NpgsqlParameter npgsqlParameter) {
            npgsqlParameter.NpgsqlDbType = NpgsqlDbType.Jsonb;
        } else {
            parameter.DbType = typedValue.DbType ?? GetDbType(typedValue.Value);
        }
        parameter.Value = typedValue.Value ?? DBNull.Value;
        return parameter;
    }

    private static DbType GetDbType(object? value) {
        if (value is Guid) {
            return DbType.Guid;
        }
        return value == null ? DbType.Object : GetDbType(Type.GetTypeCode(value.GetType()));
    }

    private static DbType GetDbType(TypeCode typeCode) {
        // no TypeCode equivalent for TimeSpan or DateTimeOffset
        switch (typeCode) {
            case TypeCode.Boolean:
                return DbType.Boolean;
            case TypeCode.Byte:
                return DbType.Byte;
            case TypeCode.Char:
                return DbType.StringFixedLength;    // ???
            case TypeCode.DateTime: // Used for Date, DateTime and DateTime2 DbTypes
                return DbType.DateTime;
            case TypeCode.Decimal:
                return DbType.Decimal;
            case TypeCode.Double:
                return DbType.Double;
            case TypeCode.Int16:
                return DbType.Int16;
            case TypeCode.Int32:
                return DbType.Int32;
            case TypeCode.Int64:
                return DbType.Int64;
            case TypeCode.SByte:
                return DbType.SByte;
            case TypeCode.Single:
                return DbType.Single;
            case TypeCode.String:
                return DbType.String;
            case TypeCode.UInt16:
                return DbType.UInt16;
            case TypeCode.UInt32:
                return DbType.UInt32;
            case TypeCode.UInt64:
                return DbType.UInt64;
            case TypeCode.DBNull:
            case TypeCode.Empty:
            case TypeCode.Object:
            default:
                return DbType.Object;
        }
    }
}