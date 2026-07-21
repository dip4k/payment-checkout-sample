namespace CheckoutSystem.Application.Models;

/// <summary>
/// Represents the order submission result.
/// </summary>
/// <param name="Totals">The calculated totals.</param>
/// <param name="Replay">Whether the result was replayed from idempotency storage.</param>
public sealed record OrderSubmissionResultModel(OrderCalculationResultModel Totals, bool Replay);