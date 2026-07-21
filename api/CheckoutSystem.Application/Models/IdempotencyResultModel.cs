namespace CheckoutSystem.Application.Models;

/// <summary>
/// Represents a persisted idempotent response payload.
/// </summary>
/// <param name="Key">The idempotency key.</param>
/// <param name="OperationName">The operation name.</param>
/// <param name="RequestHash">The request payload hash.</param>
/// <param name="Subtotal">The calculated subtotal.</param>
/// <param name="DiscountApplied">The applied discount amount.</param>
/// <param name="Tax">The calculated tax amount.</param>
/// <param name="Total">The calculated total amount.</param>
public sealed record IdempotencyResultModel(
    string Key,
    string OperationName,
    string RequestHash,
    decimal Subtotal,
    decimal DiscountApplied,
    decimal Tax,
    decimal Total);