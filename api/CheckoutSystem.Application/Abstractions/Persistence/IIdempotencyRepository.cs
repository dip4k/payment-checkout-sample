using CheckoutSystem.Application.Models;

namespace CheckoutSystem.Application.Abstractions.Persistence;

/// <summary>
/// Defines persistence operations for idempotent request handling.
/// </summary>
public interface IIdempotencyRepository
{
    /// <summary>
    /// Gets an idempotency record by key and operation name.
    /// </summary>
    /// <param name="key">The idempotency key.</param>
    /// <param name="operationName">The API operation name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The matching record when found.</returns>
    Task<IdempotencyResultModel?> GetAsync(string key, string operationName, CancellationToken cancellationToken);

    /// <summary>
    /// Saves a successful idempotency record if absent.
    /// </summary>
    /// <param name="record">The record to save.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The save outcome.</returns>
    Task<IdempotencySaveResultModel> SaveIfAbsentAsync(IdempotencyResultModel record, CancellationToken cancellationToken);
}