using System.Runtime.Serialization;
using CaaS.Core.Base.Validation;

namespace CaaS.Core.Base.Exceptions; 

public class CaasPaymentInsufficientCreditException : CaasPaymentException {
    protected CaasPaymentInsufficientCreditException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    public CaasPaymentInsufficientCreditException(string? message = null, Exception? innerException = null) : base(message, innerException) { }
    public CaasPaymentInsufficientCreditException(ValidationFailure error) : base(error) { }
    public CaasPaymentInsufficientCreditException(string message, IEnumerable<ValidationFailure> errors) : base(message, errors) { }
    public CaasPaymentInsufficientCreditException(IEnumerable<ValidationFailure> errors, string? message = null, Exception? innerException = null) : base(errors, message, innerException) { }
}