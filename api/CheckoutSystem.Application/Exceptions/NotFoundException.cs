namespace CheckoutSystem.Application.Exceptions;

/// <summary>
/// Represents an exception for missing resources.
/// </summary>
public sealed class NotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public NotFoundException(string message)
        : base(message)
    {
    }
}