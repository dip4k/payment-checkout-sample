namespace CheckoutSystem.Api.Contracts.V1;

/// <summary>
/// Represents a request to calculate order totals.
/// </summary>
/// <param name="LineItems">The requested order line items.</param>
/// <param name="Discount">An optional order-level discount.</param>
public sealed record CalculateOrderRequest(IReadOnlyCollection<OrderLineRequest> LineItems, DiscountRequest? Discount);

/// <summary>
/// Represents a line item in an order request.
/// </summary>
/// <param name="ProductId">The product identifier.</param>
/// <param name="Quantity">The item quantity.</param>
/// <param name="ProductVersion">The expected product version for optimistic concurrency.</param>
public sealed record OrderLineRequest(Guid ProductId, int Quantity, long? ProductVersion = null);

/// <summary>
/// Represents a discount request.
/// </summary>
/// <param name="Type">The discount type.</param>
/// <param name="Value">The discount value.</param>
public sealed record DiscountRequest(DiscountTypeContract Type, decimal Value);