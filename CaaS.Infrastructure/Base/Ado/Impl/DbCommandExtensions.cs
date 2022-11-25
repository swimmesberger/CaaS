using System.Data.Common;
using CaaS.Infrastructure.Base.Ado.Impl.Npgsql;
using CaaS.Infrastructure.Base.Ado.Model;

namespace CaaS.Infrastructure.Base.Ado.Impl; 

public static class DbCommandExtensions {
    public static DbParameter SetParameter(this DbParameter parameter, string name, TypedValue typedValue = default) {
        parameter.ParameterName = name;
        parameter.Value = typedValue.Value ?? DBNull.Value;
        parameter = parameter.SetParameterType(typedValue);
        return parameter;
    }
}