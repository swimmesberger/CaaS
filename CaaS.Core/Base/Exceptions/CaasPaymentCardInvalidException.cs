using System.Runtime.Serialization;
using CaaS.Core.Base.Validation;
namespace CaaS.Core.Base.Exceptions; 

public class CaasPaymentCardInvalidException : CaasPaymentException {
    protected CaasPaymentCardInvalidException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    public CaasPaymentCardInvalidException(string? message = null, Exception? innerException = null) : base(message, innerException) { }
    public CaasPaymentCardInvalidException(ValidationFailure error) : base(error) { }
    public CaasPaymentCardInvalidException(string message, IEnumerable<ValidationFailure> errors) : base(message, errors) { }
    public CaasPaymentCardInvalidException(IEnumerable<ValidationFailure> errors, string? message = null, Exception? innerException = null) : base(errors, message, innerException) { }
}