using CheckoutSystem.Api.Contracts.V1;
using CheckoutSystem.Application.Abstractions.Commands;
using CheckoutSystem.Application.Abstractions.Services;
using CheckoutSystem.Application.Commands;
using CheckoutSystem.Application.Models;
using CheckoutSystem.Domain;
using Microsoft.AspNetCore.Mvc;

namespace CheckoutSystem.Api.Endpoints.V1;

internal static class CheckoutEndpoints
{
    public static IEndpointRouteBuilder MapCheckoutEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1");

        group.MapGet("/catalogue", async (ICheckoutService checkoutService, CancellationToken cancellationToken) =>
            {
                var items = await checkoutService.GetCatalogueAsync(cancellationToken);
                var response = items
                    .Select(item => new CatalogueItemResponse(item.Id, item.Name, item.UnitPrice, item.IsTaxable, item.Version))
                    .ToArray();

                return Results.Ok(response);
            })
            .WithName("GetCatalogue")
            .WithCatalogueDocumentation();

        group.MapPost("/orders/calculate", async (
                CalculateOrderRequest request,
                ICheckoutService checkoutService,
                CancellationToken cancellationToken) =>
            {
                var appRequest = new OrderCalculationRequestModel(
                    request.LineItems
                        .Select(lineItem => new OrderLineItemModel(lineItem.ProductId, lineItem.Quantity, lineItem.ProductVersion))
                        .ToArray(),
                    request.Discount is null
                        ? null
                        : new DiscountModel(MapDiscountType(request.Discount.Type), request.Discount.Value));

                var result = await checkoutService.CalculateOrderAsync(appRequest, cancellationToken);
                    var response = MapCalculationResponse(result);

                return Results.Ok(response);
            })
            .WithName("CalculateOrderTotals")
            .WithCalculateOrderDocumentation();

        group.MapPost("/orders/submit", async (
                HttpContext httpContext,
                CalculateOrderRequest request,
                ISubmitOrderCommandHandler submitOrderCommandHandler,
                CancellationToken cancellationToken) =>
            {
                if (!httpContext.Request.Headers.TryGetValue("Idempotency-Key", out var idempotencyKeyValues)
                    || string.IsNullOrWhiteSpace(idempotencyKeyValues))
                {
                    return Results.BadRequest(new ProblemDetails
                    {
                        Title = "Validation failed.",
                        Detail = "Idempotency-Key header is required for order submission.",
                        Status = StatusCodes.Status400BadRequest,
                    });
                }

                var idempotencyKey = idempotencyKeyValues.ToString().Trim();
                var correlationId = httpContext.Items.TryGetValue("X-Correlation-ID", out var value)
                    ? value?.ToString() ?? string.Empty
                    : string.Empty;

                var appRequest = new OrderCalculationRequestModel(
                    request.LineItems
                        .Select(lineItem => new OrderLineItemModel(lineItem.ProductId, lineItem.Quantity, lineItem.ProductVersion))
                        .ToArray(),
                    request.Discount is null
                        ? null
                        : new DiscountModel(MapDiscountType(request.Discount.Type), request.Discount.Value));

                var submissionResult = await submitOrderCommandHandler.HandleAsync(
                    new SubmitOrderCommand(idempotencyKey, correlationId, appRequest),
                    cancellationToken);

                var result = submissionResult.Totals;
                var response = MapCalculationResponse(result);
                return Results.Ok(response);
            })
            .WithName("SubmitOrder")
            .WithSubmitOrderDocumentation();

        return endpoints;
    }

    private static DiscountType MapDiscountType(DiscountTypeContract contract)
    {
        return contract switch
        {
            DiscountTypeContract.None => DiscountType.None,
            DiscountTypeContract.Percentage => DiscountType.Percentage,
            DiscountTypeContract.FixedAmount => DiscountType.FixedAmount,
            _ => DiscountType.None,
        };
    }

    private static CalculateOrderResponse MapCalculationResponse(OrderCalculationResultModel result)
    {
        return new CalculateOrderResponse(
            result.Subtotal,
            result.DiscountApplied,
            result.Tax,
            result.Total,
            result.SplitShares
                .Select(share => new SplitPaymentShareResponse(share.Payer, share.Amount))
                .ToArray());
    }
}