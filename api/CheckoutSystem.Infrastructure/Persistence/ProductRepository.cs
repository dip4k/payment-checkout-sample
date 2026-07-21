using CheckoutSystem.Application.Abstractions.Persistence;
using CheckoutSystem.Domain;
using Microsoft.EntityFrameworkCore;

namespace CheckoutSystem.Infrastructure.Persistence;

/// <summary>
/// Implements product persistence operations.
/// </summary>
public sealed class ProductRepository : IProductRepository
{
    private readonly CheckoutDbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductRepository"/> class.
    /// </summary>
    /// <param name="dbContext">The db context.</param>
    public ProductRepository(CheckoutDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<Product>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Products
            .AsNoTracking()
            .OrderBy(product => product.Name)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<Product>> GetByIdsAsync(IReadOnlyCollection<Guid> productIds, CancellationToken cancellationToken)
    {
        return await _dbContext.Products
            .AsNoTracking()
            .Where(product => productIds.Contains(product.Id))
            .ToListAsync(cancellationToken);
    }
}