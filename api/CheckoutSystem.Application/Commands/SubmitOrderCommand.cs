using CheckoutSystem.Application.Models;

namespace CheckoutSystem.Application.Commands;

/// <summary>
/// Represents a submit-order command.
/// </summary>
/// <param name="IdempotencyKey">The idempotency key.</param>
/// <param name="CorrelationId">The correlation id.</param>
/// <param name="Request">The order calculation request.</param>
public sealed record SubmitOrderCommand(
    string IdempotencyKey,
    string CorrelationId,
    OrderCalculationRequestModel Request);