using CaaS.Core.Base.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CaaS.Api.Base; 

public sealed class WebValidator : IValidator {
    private readonly IObjectModelValidator _modelValidator;

    public WebValidator(IObjectModelValidator modelValidator) {
        _modelValidator = modelValidator;
    }

    public bool TryValidateModel(object model) {
        if (model == null) {
            throw new ArgumentNullException(nameof(model));
        }
        var actionContext = new ActionContext();
        _modelValidator.Validate(
            actionContext,
            validationState: null,
            prefix:  string.Empty,
            model: model);
        return actionContext.ModelState.IsValid;
    }
}