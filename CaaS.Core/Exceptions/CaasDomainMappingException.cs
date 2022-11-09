using System.Runtime.Serialization;

namespace CaaS.Core.Exceptions; 

public class CaasDomainMappingException : CaasException {
    public CaasDomainMappingException() { }
    protected CaasDomainMappingException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    public CaasDomainMappingException(string? message) : base(message) { }
    public CaasDomainMappingException(string? message, Exception? innerException) : base(message, innerException) { }
}