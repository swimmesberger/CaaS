using System.Diagnostics.CodeAnalysis;

namespace CaaS.Core.Request; 

public interface IRequestDataAccessor {
    bool TryGetTenant([MaybeNullWhen(false)] out string tenantId);
}