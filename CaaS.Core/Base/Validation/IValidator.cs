namespace CaaS.Core.Base.Validation;

public interface IValidator {
    bool TryValidateModel(object model);
}