using System.Data.Common;
using CaaS.Infrastructure.Base.Ado.Model;
using Npgsql;
using NpgsqlTypes;

namespace CaaS.Infrastructure.Base.Ado.Impl.Npgsql; 

public static class NpgsqlExtensions {
    public static DbParameter SetParameterType(this DbParameter parameter, TypedValue typedValue = default) {
        if (typedValue.IsJson && parameter is NpgsqlParameter npgsqlParameter) {
            npgsqlParameter.NpgsqlDbType = NpgsqlDbType.Jsonb;
        } else {
            parameter.DbType = typedValue.DbType ?? DbTypeUtil.GetDbType(typedValue.Value);
        }
        return parameter;
    }
}