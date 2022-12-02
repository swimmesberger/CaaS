using System.Runtime.Serialization;

namespace SwKo.Pay.Api.Exceptions; 

public class SwKoPayCannotRefundConfirmedPayment : SwKoPayException {
    public SwKoPayCannotRefundConfirmedPayment() { }

    protected SwKoPayCannotRefundConfirmedPayment(SerializationInfo info, StreamingContext context) : base(info, context) { }

    public SwKoPayCannotRefundConfirmedPayment(string? message) : base(message) { }

    public SwKoPayCannotRefundConfirmedPayment(string? message, Exception? innerException) : base(message, innerException) { }
}