using System.Collections.Immutable;
using System.Runtime.Serialization;
using CaaS.Core.Base.Validation;
namespace CaaS.Core.Base.Exceptions; 

public class CaasPaymentCardInvalidException : CaasException {
    public IEnumerable<ValidationFailure> Errors { get; private set; } = ImmutableArray<ValidationFailure>.Empty;
    
    public CaasPaymentCardInvalidException() { }
    protected CaasPaymentCardInvalidException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    public CaasPaymentCardInvalidException(string? message) : base(message) { }
    public CaasPaymentCardInvalidException(string? message, Exception? innerException) : base(message, innerException) { }

    public CaasPaymentCardInvalidException(IEnumerable<ValidationFailure> errors) {
        Errors = errors;
    }
    
    public CaasPaymentCardInvalidException(ValidationFailure error) {
        Errors = new ValidationFailure[]{error};
    }
    
    public CaasPaymentCardInvalidException(string message, IEnumerable<ValidationFailure> errors) : base(message) {
        Errors = errors;
    }
}