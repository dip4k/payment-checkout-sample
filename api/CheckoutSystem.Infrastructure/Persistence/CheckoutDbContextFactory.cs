using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CheckoutSystem.Infrastructure.Persistence;

/// <summary>
/// Creates <see cref="CheckoutDbContext"/> for design-time EF Core tooling.
/// </summary>
public sealed class CheckoutDbContextFactory : IDesignTimeDbContextFactory<CheckoutDbContext>
{
    /// <inheritdoc/>
    public CheckoutDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CheckoutDbContext>();
        optionsBuilder.UseSqlite("Data Source=checkout-system.dev.db");
        return new CheckoutDbContext(optionsBuilder.Options);
    }
}