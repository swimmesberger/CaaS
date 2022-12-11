using CaaS.Core.Base.Validation;

namespace CaaS.Test; 

public sealed class MockValidator : IValidator {
    public bool TryValidateModel(object model) => true;
}