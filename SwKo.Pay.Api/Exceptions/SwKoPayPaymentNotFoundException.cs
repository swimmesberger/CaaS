using System.Runtime.Serialization;

namespace SwKo.Pay.Api.Exceptions; 

public class SwKoPayPaymentNotFoundException : SwKoPayException {
    public SwKoPayPaymentNotFoundException() { }

    protected SwKoPayPaymentNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    public SwKoPayPaymentNotFoundException(string? message) : base(message) { }

    public SwKoPayPaymentNotFoundException(string? message, Exception? innerException) : base(message, innerException) { }
}