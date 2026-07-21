using CheckoutSystem.Application.Abstractions.Persistence;
using CheckoutSystem.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace CheckoutSystem.Infrastructure.Persistence;

/// <summary>
/// Implements idempotency record persistence operations.
/// </summary>
public sealed class IdempotencyRepository : IIdempotencyRepository
{
    private readonly CheckoutDbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="IdempotencyRepository"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    public IdempotencyRepository(CheckoutDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc/>
    public async Task<IdempotencyResultModel?> GetAsync(string key, string operationName, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.IdempotencyRecords
            .AsNoTracking()
            .SingleOrDefaultAsync(
                record => record.Key == key && record.OperationName == operationName,
                cancellationToken);

        if (entity is null)
        {
            return null;
        }

        return new IdempotencyResultModel(
            entity.Key,
            entity.OperationName,
            entity.RequestHash,
            entity.Subtotal,
            entity.DiscountApplied,
            entity.Tax,
            entity.Total);
    }

    /// <inheritdoc/>
    public async Task<IdempotencySaveResultModel> SaveIfAbsentAsync(IdempotencyResultModel record, CancellationToken cancellationToken)
    {
        var entity = new IdempotencyRecord
        {
            Id = Guid.NewGuid(),
            Key = record.Key,
            OperationName = record.OperationName,
            RequestHash = record.RequestHash,
            Subtotal = record.Subtotal,
            DiscountApplied = record.DiscountApplied,
            Tax = record.Tax,
            Total = record.Total,
            CreatedAtUtc = DateTimeOffset.UtcNow,
        };

        _dbContext.IdempotencyRecords.Add(entity);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            return new IdempotencySaveResultModel(IdempotencySaveStatus.Created);
        }
        catch (DbUpdateException)
        {
            _dbContext.Entry(entity).State = EntityState.Detached;

            var existing = await GetAsync(record.Key, record.OperationName, cancellationToken);
            if (existing is null)
            {
                throw;
            }

            if (string.Equals(existing.RequestHash, record.RequestHash, StringComparison.Ordinal))
            {
                return new IdempotencySaveResultModel(IdempotencySaveStatus.ExistingSameRequest, existing);
            }

            return new IdempotencySaveResultModel(IdempotencySaveStatus.ExistingDifferentRequest, existing);
        }
    }
}