using CheckoutSystem.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CheckoutSystem.Infrastructure.Persistence;

/// <summary>
/// Implements transactional execution against the checkout database context.
/// </summary>
public sealed class EfUnitOfWork : IUnitOfWork
{
    private readonly CheckoutDbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="EfUnitOfWork"/> class.
    /// </summary>
    /// <param name="dbContext">The db context.</param>
    public EfUnitOfWork(CheckoutDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc/>
    public async Task<TResult> ExecuteInTransactionAsync<TResult>(
        Func<CancellationToken, Task<TResult>> operation,
        CancellationToken cancellationToken)
    {
        var strategy = _dbContext.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var result = await operation(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return result;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }
}