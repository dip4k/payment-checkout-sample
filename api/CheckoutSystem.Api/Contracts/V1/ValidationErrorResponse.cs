namespace CheckoutSystem.Api.Contracts.V1;

/// <summary>
/// Represents a structured validation error item.
/// </summary>
/// <param name="Code">The error code.</param>
/// <param name="Field">The field name.</param>
/// <param name="Message">The validation message.</param>
public sealed record ValidationErrorResponse(string Code, string Field, string Message);