namespace CheckoutSystem.Application.Validation;

/// <summary>
/// Represents a single validation error.
/// </summary>
/// <param name="Code">The error code.</param>
/// <param name="Field">The field associated with the error.</param>
/// <param name="Message">The validation message.</param>
public sealed record ValidationError(string Code, string Field, string Message);

/// <summary>
/// Represents the outcome of model validation.
/// </summary>
public sealed class ValidationResult
{
    private readonly List<ValidationError> _errors = [];

    /// <summary>
    /// Gets the validation errors.
    /// </summary>
    public IReadOnlyCollection<ValidationError> Errors => _errors;

    /// <summary>
    /// Gets a value indicating whether the result is valid.
    /// </summary>
    public bool IsValid => _errors.Count == 0;

    /// <summary>
    /// Adds a validation error to the result.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="field">The field name.</param>
    /// <param name="message">The error message.</param>
    public void AddError(string code, string field, string message)
    {
        _errors.Add(new ValidationError(code, field, message));
    }
}