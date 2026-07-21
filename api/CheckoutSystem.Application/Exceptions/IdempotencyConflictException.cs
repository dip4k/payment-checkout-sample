namespace CheckoutSystem.Application.Exceptions;

/// <summary>
/// Represents an idempotency conflict where a key is reused with a different payload.
/// </summary>
public sealed class IdempotencyConflictException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IdempotencyConflictException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public IdempotencyConflictException(string message)
        : base(message)
    {
    }
}