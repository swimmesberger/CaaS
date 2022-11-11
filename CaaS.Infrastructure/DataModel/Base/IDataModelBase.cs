﻿using CaaS.Core;

namespace CaaS.Infrastructure.DataModel.Base; 

public interface IDataModelBase : IHasId {
    // optimistic locking
    int RowVersion { get; } 
    
    // auditing, can be used as clustered index in SQL Server for performance reasons
    // Postgres has a UUID Datatype that can be stored optimal
    DateTimeOffset CreationTime { get; }
    DateTimeOffset LastModificationTime { get; }
}