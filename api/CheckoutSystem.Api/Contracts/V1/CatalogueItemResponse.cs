namespace CheckoutSystem.Api.Contracts.V1;

/// <summary>
/// Represents a catalogue item response.
/// </summary>
/// <param name="Id">The product identifier.</param>
/// <param name="Name">The product name.</param>
/// <param name="UnitPrice">The product unit price.</param>
/// <param name="IsTaxable">Whether the product is taxable.</param>
/// <param name="Version">The optimistic concurrency version.</param>
public sealed record CatalogueItemResponse(Guid Id, string Name, decimal UnitPrice, bool IsTaxable, long Version);