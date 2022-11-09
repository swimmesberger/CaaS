using System.Data.Common;

namespace CaaS.Infrastructure.Ado.Base; 

public delegate ValueTask<T> RowMapper<T>(DbDataReader record, CancellationToken cancellationToken = default);