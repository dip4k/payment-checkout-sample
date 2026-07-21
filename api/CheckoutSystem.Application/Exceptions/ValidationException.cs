using CheckoutSystem.Application.Validation;

namespace CheckoutSystem.Application.Exceptions;

/// <summary>
/// Represents a validation exception with structured error details.
/// </summary>
public sealed class ValidationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class.
    /// </summary>
    /// <param name="errors">The validation errors.</param>
    public ValidationException(IReadOnlyCollection<ValidationError> errors)
        : base("Validation failed.")
    {
        Errors = errors;
    }

    /// <summary>
    /// Gets the validation errors.
    /// </summary>
    public IReadOnlyCollection<ValidationError> Errors { get; }
}