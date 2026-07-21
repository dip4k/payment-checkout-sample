namespace CheckoutSystem.Infrastructure.Persistence;

/// <summary>
/// Represents a persisted idempotent response record.
/// </summary>
public sealed class IdempotencyRecord
{
    /// <summary>
    /// Gets or sets the record identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the idempotency key value.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the operation name.
    /// </summary>
    public string OperationName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the request hash.
    /// </summary>
    public string RequestHash { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the subtotal.
    /// </summary>
    public decimal Subtotal { get; set; }

    /// <summary>
    /// Gets or sets the applied discount amount.
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
}