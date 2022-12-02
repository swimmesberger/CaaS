using System.Runtime.Serialization;

namespace SwKo.Pay.Api.Exceptions; 

//e.g. when card was reported as lost or expired
public class SwKoPayCardInactiveException : SwKoPayException {
    public SwKoPayCardInactiveException() { }

    protected SwKoPayCardInactiveException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    public SwKoPayCardInactiveException(string? message) : base(message) { }

    public SwKoPayCardInactiveException(string? message, Exception? innerException) : base(message, innerException) { }
}