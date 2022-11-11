using System.Runtime.Serialization;

namespace CaaS.Core.Base.Exceptions; 

public class CaasException : Exception {
    public CaasException() { }
    protected CaasException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    public CaasException(string? message) : base(message) { }
    public CaasException(string? message, Exception? innerException) : base(message, innerException) { }
}