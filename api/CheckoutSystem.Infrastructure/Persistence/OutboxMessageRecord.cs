namespace CheckoutSystem.Infrastructure.Persistence;

/// <summary>
/// Represents an outbox message ready for integration publishing.
/// </summary>
public sealed class OutboxMessageRecord
{
    /// <summary>
    /// Gets or sets the message identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the event type.
    /// </summary>
    public string EventType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the aggregate identifier.
    /// </summary>
    public string AggregateId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the event payload JSON.
    /// </summary>
    public string Payload { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the creation timestamp.
    /// </summary>
    public DateTimeOffset CreatedAtUtc { get; set; }

    /// <summary>
    /// Gets or sets the processing timestamp.
    /// </summary>
    public DateTimeOffset? ProcessedAtUtc { get; set; }
}