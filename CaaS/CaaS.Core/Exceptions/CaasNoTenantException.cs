using System.Runtime.Serialization;

namespace CaaS.Core.Exceptions; 

public class CaasNoTenantException : CaasException {
    public CaasNoTenantException() { }
    protected CaasNoTenantException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    public CaasNoTenantException(string? message) : base(message) { }
    public CaasNoTenantException(string? message, Exception? innerException) : base(message, innerException) { }
}