namespace CaaS.Core.Entities.Base; 

public interface IEntityBase : IHasId {
    // must be passed through, detects concurrent updates
    string ConcurrencyToken { get; }
}