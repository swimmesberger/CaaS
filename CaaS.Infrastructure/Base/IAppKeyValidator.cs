namespace CaaS.Infrastructure.Base; 

public interface IAppKeyValidator {
    Task<bool> ValidateAppKeyAsync(string appKey, CancellationToken cancellationToken = default);
}