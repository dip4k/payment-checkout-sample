namespace CheckoutSystem.Domain;

/// <summary>
/// Represents discount types that can be applied to an order.
/// </summary>
public enum DiscountType
{
    /// <summary>
    /// No discount is applied.
    /// </summary>
    None = 0,

    /// <summary>
    /// Percentage discount.
    /// </summary>
    Percentage = 1,

    /// <summary>
    /// Fixed amount discount.
    /// </summary>
    FixedAmount = 2,
}