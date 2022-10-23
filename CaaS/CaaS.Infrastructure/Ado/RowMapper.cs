using System.Data;

namespace CaaS.Infrastructure.Ado; 

public delegate T RowMapper<out T>(IDataRecord record);