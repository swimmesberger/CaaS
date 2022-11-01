namespace CaaS.Core.Entities.Base; 

public interface IEntityBase {
    Guid Id { get; }
    
    // must be passed through, detects concurrent updates
    string ConcurrencyToken { get; }
}