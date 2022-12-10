using System.Collections.Immutable;
using System.Runtime.Serialization;
using CaaS.Core.Base.Validation;
namespace CaaS.Core.Base.Exceptions; 

public class CaasValidationException : CaasException {
    public IEnumerable<ValidationFailure> Errors { get; private set; } = ImmutableArray<ValidationFailure>.Empty;
    
    public CaasValidationException() { }
    protected CaasValidationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    public CaasValidationException(string? message) : base(message) { }
    public CaasValidationException(string? message, Exception? innerException) : base(message, innerException) { }

    public CaasValidationException(IEnumerable<ValidationFailure> errors) {
        Errors = errors;
    }
    
    public CaasValidationException(ValidationFailure error) {
        Errors = new ValidationFailure[]{error};
    }
    
    public CaasValidationException(string message, IEnumerable<ValidationFailure> errors) : base(message) {
        Errors = errors;
    }
}