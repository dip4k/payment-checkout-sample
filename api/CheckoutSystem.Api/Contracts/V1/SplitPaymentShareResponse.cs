namespace CheckoutSystem.Api.Contracts.V1;

/// <summary>
/// Represents a single payment share in the Part Two split response.
/// </summary>
/// <param name="Payer">The 1-based payer number.</param>
/// <param name="Amount">The amount assigned to the payer.</param>
public sealed record SplitPaymentShareResponse(int Payer, decimal Amount);