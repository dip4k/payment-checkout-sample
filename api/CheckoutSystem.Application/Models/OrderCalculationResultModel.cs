namespace CheckoutSystem.Application.Models;

/// <summary>
/// Represents the order totals breakdown result.
/// </summary>
/// <param name="Subtotal">The pre-discount subtotal.</param>
/// <param name="DiscountApplied">The applied discount amount.</param>
/// <param name="Tax">The calculated tax amount.</param>
/// <param name="Total">The final order total.</param>
/// <param name="SplitShares">The three-way split derived from the final order total.</param>
public sealed record OrderCalculationResultModel(
	decimal Subtotal,
	decimal DiscountApplied,
	decimal Tax,
	decimal Total,
	IReadOnlyCollection<OrderSplitShareModel> SplitShares);