using System.Data.Common;

namespace CaaS.Infrastructure.Base.Ado; 

public delegate ValueTask<T> RowMapper<T>(DbDataReader record, CancellationToken cancellationToken = default);