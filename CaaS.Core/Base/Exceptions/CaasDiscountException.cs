using System.Runtime.Serialization;

namespace CaaS.Core.Base.Exceptions; 

public class CaasDiscountException : CaasException {
    public CaasDiscountException() { }
    protected CaasDiscountException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    public CaasDiscountException(string? message) : base(message) { }
    public CaasDiscountException(string? message, Exception? innerException) : base(message, innerException) { }
}