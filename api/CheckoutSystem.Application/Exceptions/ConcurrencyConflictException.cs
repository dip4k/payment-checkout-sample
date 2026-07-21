namespace CheckoutSystem.Application.Exceptions;

/// <summary>
/// Represents an optimistic concurrency conflict.
/// </summary>
public sealed class ConcurrencyConflictException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConcurrencyConflictException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public ConcurrencyConflictException(string message)
        : base(message)
    {
    }
}