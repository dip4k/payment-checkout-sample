namespace CheckoutSystem.Api.Contracts.V1;

/// <summary>
/// Represents the order calculation response.
/// </summary>
/// <param name="Subtotal">The pre-discount subtotal.</param>
/// <param name="DiscountApplied">The applied discount amount.</param>
/// <param name="Tax">The calculated tax amount.</param>
/// <param name="Total">The final total amount.</param>
/// <param name="SplitShares">The three-way split derived from the final order total.</param>
public sealed record CalculateOrderResponse(
	decimal Subtotal,
	decimal DiscountApplied,
	decimal Tax,
	decimal Total,
	IReadOnlyCollection<SplitPaymentShareResponse> SplitShares);