using System.Runtime.Serialization;

namespace CaaS.Core.Exceptions; 

public class CaasDbException : CaasException {
    public CaasDbException() { }
    protected CaasDbException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    public CaasDbException(string? message) : base(message) { }
    public CaasDbException(string? message, Exception? innerException) : base(message, innerException) { }
}