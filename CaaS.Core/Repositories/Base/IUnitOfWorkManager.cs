namespace CaaS.Core.Repositories.Base; 

public interface IUnitOfWorkManager {
    /// <summary>
    /// Gets currently active unit of work (or null if not exists).
    /// </summary>
    IUnitOfWork? Current { get; }

    /// <summary>
    /// Begins a new unit of work.
    /// </summary>
    /// <returns>A handle to be able to complete the unit of work</returns>
    IUnitOfWork Begin();
}