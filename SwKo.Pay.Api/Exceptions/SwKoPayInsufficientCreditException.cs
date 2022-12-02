using System.Runtime.Serialization;

namespace SwKo.Pay.Api.Exceptions; 

public class SwKoPayInsufficientCreditException : SwKoPayException {
    public SwKoPayInsufficientCreditException() { }

    protected SwKoPayInsufficientCreditException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    public SwKoPayInsufficientCreditException(string? message) : base(message) { }

    public SwKoPayInsufficientCreditException(string? message, Exception? innerException) : base(message, innerException) { }
}