namespace CheckoutSystem.Application.Models;

/// <summary>
/// Represents a single payment share derived from the final order total.
/// </summary>
/// <param name="Payer">The 1-based payer number.</param>
/// <param name="Amount">The amount assigned to the payer.</param>
public sealed record OrderSplitShareModel(int Payer, decimal Amount);