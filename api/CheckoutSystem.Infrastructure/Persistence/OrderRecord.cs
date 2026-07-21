namespace CheckoutSystem.Infrastructure.Persistence;

/// <summary>
/// Represents a submitted order persistence record.
/// </summary>
public sealed class OrderRecord
{
    /// <summary>
    /// Gets or sets the order identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the idempotency key.
    /// </summary>
    public string IdempotencyKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the correlation id.
    /// </summary>
    public string CorrelationId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the subtotal amount.
    /// </summary>
    public decimal Subtotal { get; set; }

    /// <summary>
    /// Gets or sets the discount amount.
    /// </summary>
    public decimal DiscountApplied { get; set; }

    /// <summary>
    /// Gets or sets the tax amount.
    /// </summary>
    public decimal Tax { get; set; }

    /// <summary>
    /// Gets or sets the total amount.
    /// </summary>
    public decimal Total { get; set; }

    /// <summary>
    /// Gets or sets the creation timestamp.
    /// </summary>
    public DateTimeOffset CreatedAtUtc { get; set; }

    /// <summary>
    /// Gets or sets the immutable line snapshots.
    /// </summary>
    public ICollection<OrderLineSnapshotRecord> LineSnapshots { get; set; } = [];

    /// <summary>
    /// Gets or sets the order status history entries.
    /// </summary>
    public ICollection<OrderStatusHistoryRecord> StatusHistory { get; set; } = [];
}