namespace CheckoutSystem.Application.Abstractions.Events;

/// <summary>
/// Represents a domain event marker interface.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Gets the timestamp when the event occurred.
    /// </summary>
    DateTimeOffset OccurredAtUtc { get; }
}