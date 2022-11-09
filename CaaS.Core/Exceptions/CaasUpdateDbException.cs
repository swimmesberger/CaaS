using System.Runtime.Serialization;

namespace CaaS.Core.Exceptions; 

public class CaasUpdateDbException : CaasDbException {
    public CaasUpdateDbException() { }
    protected CaasUpdateDbException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    public CaasUpdateDbException(string? message) : base(message) { }
    public CaasUpdateDbException(string? message, Exception? innerException) : base(message, innerException) { }
}