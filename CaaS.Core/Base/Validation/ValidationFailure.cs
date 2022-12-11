namespace CaaS.Core.Base.Validation;

/// <summary>
/// Defines a validation failure
/// </summary>
[Serializable]
public class ValidationFailure {

	/// <summary>
	/// Creates a new validation failure.
	/// </summary>
	public ValidationFailure() {

	}

	/// <summary>
	/// Creates a new validation failure.
	/// </summary>
	public ValidationFailure(string propertyName, string errorMessage) : this(propertyName, errorMessage, null) {

	}

	/// <summary>
	/// Creates a new ValidationFailure.
	/// </summary>
	public ValidationFailure(string propertyName, string errorMessage, object attemptedValue) {
		PropertyName = propertyName;
		ErrorMessage = errorMessage;
		AttemptedValue = attemptedValue;
	}

	/// <summary>
	/// The name of the property.
	/// </summary>
	public string PropertyName { get; set; }

	/// <summary>
	/// The error message
	/// </summary>
	public string ErrorMessage { get; set; }

	/// <summary>
	/// The property value that caused the failure.
	/// </summary>
	public object AttemptedValue { get; set; }

	/// <summary>
	/// Custom state associated with the failure.
	/// </summary>
	public object CustomState { get; set; }

	/// <summary>
	/// Custom severity level associated with the failure.
	/// </summary>
	public Severity Severity { get; set; } = Severity.Error;

	/// <summary>
	/// Gets or sets the error code.
	/// </summary>
	public string ErrorCode { get; set; }

	/// <summary>
	/// Gets or sets the formatted message placeholder values.
	/// </summary>
	public Dictionary<string, object> FormattedMessagePlaceholderValues { get; set; }

	/// <summary>
	/// Creates a textual representation of the failure.
	/// </summary>
	public override string ToString() {
		return ErrorMessage;
	}
}