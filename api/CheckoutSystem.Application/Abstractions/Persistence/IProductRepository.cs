using CheckoutSystem.Domain;

namespace CheckoutSystem.Application.Abstractions.Persistence;

/// <summary>
/// Defines product data access operations.
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// Gets all products in the catalogue.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The product list.</returns>
    Task<IReadOnlyCollection<Product>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets products by identifier set.
    /// </summary>
    /// <param name="productIds">The product identifiers.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The matching products.</returns>
    Task<IReadOnlyCollection<Product>> GetByIdsAsync(IReadOnlyCollection<Guid> productIds, CancellationToken cancellationToken);
}