using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CheckoutSystem.Infrastructure.Persistence;

/// <summary>
/// Initializes the application database for local development.
/// </summary>
public static class DatabaseInitializer
{
    /// <summary>
    /// Ensures all database migrations are applied.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task InitializeAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CheckoutDbContext>();
        await dbContext.Database.MigrateAsync(cancellationToken);
    }
}