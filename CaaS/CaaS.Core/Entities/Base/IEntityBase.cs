namespace CaaS.Core.Entities.Base; 

public interface IEntityBase {
    // guid to ensure we have a artificial primary key for each entity
    Guid Id { get; }
    
    // optimistic locking
    int RowVersion { get; } 
    
    // auditing, can be used as clustered index in SQL Server for performance reasons
    // Postgres has a UUID Datatype that can be stored optimal
    DateTimeOffset CreationTime { get; }
    DateTimeOffset LastModificationTime { get; }
}