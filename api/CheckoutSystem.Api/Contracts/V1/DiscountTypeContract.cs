namespace CheckoutSystem.Api.Contracts.V1;

/// <summary>
/// Represents discount types in API contracts.
/// </summary>
public enum DiscountTypeContract
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