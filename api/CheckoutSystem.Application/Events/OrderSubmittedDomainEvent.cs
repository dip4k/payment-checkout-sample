using CheckoutSystem.Application.Abstractions.Events;

namespace CheckoutSystem.Application.Events;

/// <summary>
/// Represents a submitted order event with immutable snapshot details.
/// </summary>
public sealed class OrderSubmittedDomainEvent : IDomainEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OrderSubmittedDomainEvent"/> class.
    /// </summary>
    /// <param name="orderId">The order identifier.</param>
    /// <param name="idempotencyKey">The idempotency key.</param>
    /// <param name="correlationId">The correlation id.</param>
    /// <param name="subtotal">The subtotal.</param>
    /// <param name="discountApplied">The applied discount.</param>
    /// <param name="tax">The tax amount.</param>
    /// <param name="total">The total amount.</param>
    /// <param name="lineSnapshots">The immutable line snapshots.</param>
    public OrderSubmittedDomainEvent(
        Guid orderId,
        string idempotencyKey,
        string correlationId,
        decimal subtotal,
        decimal discountApplied,
        decimal tax,
        decimal total,
        IReadOnlyCollection<OrderLineSnapshotModel> lineSnapshots)
    {
        OrderId = orderId;
        IdempotencyKey = idempotencyKey;
        CorrelationId = correlationId;
        Subtotal = subtotal;
        DiscountApplied = discountApplied;
        Tax = tax;
        Total = total;
        LineSnapshots = lineSnapshots;
        OccurredAtUtc = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Gets the order identifier.
    /// </summary>
    public Guid OrderId { get; }

    /// <summary>
    /// Gets the idempotency key.
    /// </summary>
    public string IdempotencyKey { get; }

    /// <summary>
    /// Gets the correlation id.
    /// </summary>
    public string CorrelationId { get; }

    /// <summary>
    /// Gets the subtotal amount.
    /// </summary>
    public decimal Subtotal { get; }

    /// <summary>
    /// Gets the applied discount amount.
    /// </summary>
    public decimal DiscountApplied { get; }

    /// <summary>
    /// Gets the tax amount.
    /// </summary>
    public decimal Tax { get; }

    /// <summary>
    /// Gets the total amount.
    /// </summary>
    public decimal Total { get; }

    /// <summary>
    /// Gets immutable line snapshots.
    /// </summary>
    public IReadOnlyCollection<OrderLineSnapshotModel> LineSnapshots { get; }

    /// <inheritdoc/>
    public DateTimeOffset OccurredAtUtc { get; }
}

/// <summary>
/// Represents immutable line-item financial snapshot data.
/// </summary>
/// <param name="ProductId">The product id.</param>
/// <param name="ProductName">The product name.</param>
/// <param name="UnitPrice">The unit price.</param>
/// <param name="IsTaxable">Whether the product is taxable.</param>
/// <param name="Quantity">The quantity.</param>
/// <param name="ProductVersion">The product version.</param>
public sealed record OrderLineSnapshotModel(
    Guid ProductId,
    string ProductName,
    decimal UnitPrice,
    bool IsTaxable,
    int Quantity,
    long ProductVersion);