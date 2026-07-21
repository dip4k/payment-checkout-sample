namespace CheckoutSystem.Application.Abstractions.Events;

/// <summary>
/// Defines a handler for a domain event.
/// </summary>
/// <typeparam name="TEvent">The event type.</typeparam>
public interface IDomainEventHandler<in TEvent>
    where TEvent : IDomainEvent
{
    /// <summary>
    /// Handles the domain event.
    /// </summary>
    /// <param name="domainEvent">The domain event.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the operation.</returns>
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken);
}