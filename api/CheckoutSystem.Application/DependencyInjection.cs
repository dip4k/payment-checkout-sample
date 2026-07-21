using CheckoutSystem.Application.Abstractions.Commands;
using CheckoutSystem.Application.Abstractions.Services;
using CheckoutSystem.Application.Abstractions.Validation;
using CheckoutSystem.Application.Commands;
using CheckoutSystem.Application.Models;
using CheckoutSystem.Application.Services;
using CheckoutSystem.Application.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace CheckoutSystem.Application;

/// <summary>
/// Registers application layer services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds application layer dependencies.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IValidator<OrderCalculationRequestModel>, OrderCalculationRequestValidator>();
        services.AddScoped<ICheckoutService, CheckoutService>();
        services.AddScoped<ISubmitOrderCommandHandler, SubmitOrderCommandHandler>();
        return services;
    }
}