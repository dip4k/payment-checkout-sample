namespace CheckoutSystem.Application.Models;

/// <summary>
/// Represents the result of attempting to persist an idempotency record.
/// </summary>
/// <param name="Status">The save status.</param>
/// <param name="ExistingRecord">The existing record when available.</param>
public sealed record IdempotencySaveResultModel(
    IdempotencySaveStatus Status,
    IdempotencyResultModel? ExistingRecord = null);

/// <summary>
/// Represents possible idempotency persistence outcomes.
/// </summary>
public enum IdempotencySaveStatus
{
    /// <summary>
    /// The record was newly created.
    /// </summary>
    Created = 0,

    /// <summary>
    /// A record already exists for the same key and payload.
    /// </summary>
    ExistingSameRequest = 1,

    /// <summary>
    /// A record already exists for the same key with a different payload.
    /// </summary>
    ExistingDifferentRequest = 2,
}