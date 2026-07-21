namespace CheckoutSystem.Domain;

/// <summary>
/// Represents a catalogue product that can be added to an order.
/// </summary>
public sealed class Product
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Product"/> class.
	/// </summary>
	/// <param name="id">The unique product identifier.</param>
	/// <param name="name">The product display name.</param>
	/// <param name="unitPrice">The unit price of the product.</param>
	/// <param name="isTaxable">Whether the product is taxable.</param>
	/// <param name="version">The optimistic concurrency version.</param>
	public Product(Guid id, string name, decimal unitPrice, bool isTaxable, long version)
	{
		Id = id;
		Name = name;
		UnitPrice = unitPrice;
		IsTaxable = isTaxable;
		Version = version;
	}

	private Product()
	{
	}

	/// <summary>
	/// Gets the unique product identifier.
	/// </summary>
	public Guid Id { get; private set; }

	/// <summary>
	/// Gets the product display name.
	/// </summary>
	public string Name { get; private set; } = string.Empty;

	/// <summary>
	/// Gets the product unit price.
	/// </summary>
	public decimal UnitPrice { get; private set; }

	/// <summary>
	/// Gets a value indicating whether the product is taxable.
	/// </summary>
	public bool IsTaxable { get; private set; }

	/// <summary>
	/// Gets the optimistic concurrency version for the product.
	/// </summary>
	public long Version { get; private set; }
}