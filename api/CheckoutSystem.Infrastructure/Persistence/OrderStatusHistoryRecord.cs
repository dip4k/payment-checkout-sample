namespace CheckoutSystem.Infrastructure.Persistence;

/// <summary>
/// Represents an append-only order status history record.
/// </summary>
public sealed class OrderStatusHistoryRecord
{
    /// <summary>
    /// Gets or sets the status entry identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the order identifier.
    /// </summary>
    public Guid OrderId { get; set; }

    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the status timestamp.
    /// </summary>
    public DateTimeOffset ChangedAtUtc { get; set; }

    /// <summary>
    /// Gets or sets an optional note.
    /// </summary>
    public string Note { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the parent order.
    /// </summary>
    public OrderRecord Order { get; set; } = null!;
}