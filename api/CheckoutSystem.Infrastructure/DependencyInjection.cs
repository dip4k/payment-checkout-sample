using CheckoutSystem.Application.Abstractions.Events;
using CheckoutSystem.Application.Abstractions.Persistence;
using CheckoutSystem.Application.Events;
using CheckoutSystem.Infrastructure.Events;
using CheckoutSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CheckoutSystem.Infrastructure;

/// <summary>
/// Registers infrastructure layer services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds infrastructure layer dependencies.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("CheckoutDb")
            ?? "Data Source=checkout-system.db";

        services.AddDbContext<CheckoutDbContext>(options => options.UseSqlite(connectionString));
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IIdempotencyRepository, IdempotencyRepository>();
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddScoped<IDomainEventHandler<OrderSubmittedDomainEvent>, OrderSubmittedAuditHandler>();

        return services;
    }
}