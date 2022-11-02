using System.Data;

namespace CaaS.Infrastructure.Ado.Base; 

public delegate T RowMapper<out T>(IDataRecord record);