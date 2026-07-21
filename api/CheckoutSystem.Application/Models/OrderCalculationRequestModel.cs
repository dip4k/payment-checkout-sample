using CheckoutSystem.Domain;

namespace CheckoutSystem.Application.Models;

/// <summary>
/// Represents an order calculation request model.
/// </summary>
/// <param name="LineItems">Order line items.</param>
/// <param name="Discount">Optional order-level discount.</param>
public sealed record OrderCalculationRequestModel(
    IReadOnlyCollection<OrderLineItemModel> LineItems,
    DiscountModel? Discount);

/// <summary>
/// Represents an order line item.
/// </summary>
/// <param name="ProductId">The product identifier.</param>
/// <param name="Quantity">The item quantity.</param>
/// <param name="ProductVersion">The expected product version for optimistic concurrency.</param>
public sealed record OrderLineItemModel(Guid ProductId, int Quantity, long? ProductVersion = null);

/// <summary>
/// Represents an order-level discount.
/// </summary>
/// <param name="Type">The discount type.</param>
/// <param name="Value">The discount value.</param>
public sealed record DiscountModel(DiscountType Type, decimal Value);