using System.Runtime.Serialization;

namespace SwKo.Pay.Api.Exceptions; 

public class SwKoPayCreditCardInvalidException : SwKoPayException {
    public SwKoPayCreditCardInvalidException() { }

    protected SwKoPayCreditCardInvalidException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    public SwKoPayCreditCardInvalidException(string? message) : base(message) { }

    public SwKoPayCreditCardInvalidException(string? message, Exception? innerException) : base(message, innerException) { }
}