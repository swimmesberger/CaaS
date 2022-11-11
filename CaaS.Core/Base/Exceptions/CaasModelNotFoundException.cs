using System.Runtime.Serialization;

namespace CaaS.Core.Base.Exceptions; 

public class CaasModelNotFoundException : CaasException {
    public CaasModelNotFoundException() { }
    protected CaasModelNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    public CaasModelNotFoundException(string? message) : base(message) { }
    public CaasModelNotFoundException(string? message, Exception? innerException) : base(message, innerException) { }
}