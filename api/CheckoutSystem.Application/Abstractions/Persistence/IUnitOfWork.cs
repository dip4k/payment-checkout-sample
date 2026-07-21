namespace CheckoutSystem.Application.Abstractions.Persistence;

/// <summary>
/// Defines transaction boundary operations for application workflows.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Executes an operation within a database transaction.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="operation">The operation to execute.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The operation result.</returns>
    Task<TResult> ExecuteInTransactionAsync<TResult>(
        Func<CancellationToken, Task<TResult>> operation,
        CancellationToken cancellationToken);
}