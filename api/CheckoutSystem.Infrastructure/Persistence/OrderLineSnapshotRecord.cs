namespace CheckoutSystem.Infrastructure.Persistence;

/// <summary>
/// Represents an immutable line-item snapshot record.
/// </summary>
public sealed class OrderLineSnapshotRecord
{
    /// <summary>
    /// Gets or sets the snapshot identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the order identifier.
    /// </summary>
    public Guid OrderId { get; set; }

    /// <summary>
    /// Gets or sets the product identifier.
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Gets or sets the product name.
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the product unit price.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the item is taxable.
    /// </summary>
    public bool IsTaxable { get; set; }

    /// <summary>
    /// Gets or sets the quantity.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the product version.
    /// </summary>
    public long ProductVersion { get; set; }

    /// <summary>
    /// Gets or sets the parent order.
    /// </summary>
    public OrderRecord Order { get; set; } = null!;
}