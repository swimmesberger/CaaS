using System.Runtime.Serialization;

namespace SwKo.Pay.Api.Exceptions; 

public class SwKoPayException : Exception {
    public SwKoPayException() { }
    protected SwKoPayException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    public SwKoPayException(string? message) : base(message) { }
    public SwKoPayException(string? message, Exception? innerException) : base(message, innerException) { }
}