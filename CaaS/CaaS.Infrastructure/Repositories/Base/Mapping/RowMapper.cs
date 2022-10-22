using System.Data;

namespace CaaS.Infrastructure.Repositories.Base.Mapping; 

public delegate T RowMapper<out T>(IDataRecord record);