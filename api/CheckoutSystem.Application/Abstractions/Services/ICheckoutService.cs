using CheckoutSystem.Application.Models;

namespace CheckoutSystem.Application.Abstractions.Services;

/// <summary>
/// Defines checkout operations used by the API layer.
/// </summary>
public interface ICheckoutService
{
    /// <summary>
    /// Gets the product catalogue.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The catalogue products.</returns>
    Task<IReadOnlyCollection<CatalogueItemModel>> GetCatalogueAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Calculates order totals and returns the order-level breakdown.
    /// </summary>
    /// <param name="request">The order calculation request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The order totals result.</returns>
    Task<OrderCalculationResultModel> CalculateOrderAsync(OrderCalculationRequestModel request, CancellationToken cancellationToken);

    /// <summary>
    /// Submits an order calculation request with optimistic concurrency checks.
    /// </summary>
    /// <param name="request">The order submit request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The order totals result.</returns>
    Task<OrderCalculationResultModel> SubmitOrderAsync(OrderCalculationRequestModel request, CancellationToken cancellationToken);
}