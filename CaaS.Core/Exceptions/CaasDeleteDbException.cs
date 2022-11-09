using System.Runtime.Serialization;

namespace CaaS.Core.Exceptions; 

public class CaasDeleteDbException : CaasDbException {
    public CaasDeleteDbException() { }
    protected CaasDeleteDbException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    public CaasDeleteDbException(string? message) : base(message) { }
    public CaasDeleteDbException(string? message, Exception? innerException) : base(message, innerException) { }
}