using System.Runtime.Serialization;

namespace CaaS.Core.Base.Exceptions; 

public class CaasItemNotFoundException : CaasException {
    public CaasItemNotFoundException() { }
    protected CaasItemNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    public CaasItemNotFoundException(string? message) : base(message) { }
    public CaasItemNotFoundException(string? message, Exception? innerException) : base(message, innerException) { }
}