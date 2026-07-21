using CheckoutSystem.Application.Validation;

namespace CheckoutSystem.Application.Abstractions.Validation;

/// <summary>
/// Defines a validation contract for application models.
/// </summary>
/// <typeparam name="T">The model type to validate.</typeparam>
public interface IValidator<in T>
{
    /// <summary>
    /// Validates a model instance.
    /// </summary>
    /// <param name="model">The model to validate.</param>
    /// <returns>The validation result.</returns>
    ValidationResult Validate(T model);
}