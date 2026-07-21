using CheckoutSystem.Application.Commands;
using CheckoutSystem.Application.Models;

namespace CheckoutSystem.Application.Abstractions.Commands;

/// <summary>
/// Handles order submission commands.
/// </summary>
public interface ISubmitOrderCommandHandler
{
    /// <summary>
    /// Submits an order in an idempotent, transactional flow.
    /// </summary>
    /// <param name="command">The submit order command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The submission result.</returns>
    Task<OrderSubmissionResultModel> HandleAsync(SubmitOrderCommand command, CancellationToken cancellationToken);
}