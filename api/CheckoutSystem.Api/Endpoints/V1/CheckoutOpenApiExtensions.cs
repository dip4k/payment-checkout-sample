using CheckoutSystem.Api.Contracts.V1;
using Microsoft.AspNetCore.Mvc;

namespace CheckoutSystem.Api.Endpoints.V1;

internal static class CheckoutOpenApiExtensions
{
    public static RouteHandlerBuilder WithCatalogueDocumentation(this RouteHandlerBuilder builder)
    {
        return builder
            .WithSummary("Get the seeded checkout catalogue.")
            .WithDescription("Returns the seeded product catalogue used by the UI and API clients. Example usage: `GET /api/v1/catalogue`")
            .Produces<CatalogueItemResponse[]>(StatusCodes.Status200OK);
    }

    public static RouteHandlerBuilder WithCalculateOrderDocumentation(this RouteHandlerBuilder builder)
    {
        return builder
            .WithSummary("Calculate order totals.")
            .WithDescription("Calculates the current order using the submitted product versions and optional order-level discount.")
            .Accepts<CalculateOrderRequest>("application/json")
            .Produces<CalculateOrderResponse>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict);
    }

    public static RouteHandlerBuilder WithSubmitOrderDocumentation(this RouteHandlerBuilder builder)
    {
        return builder
            .WithSummary("Submit an idempotent order.")
            .WithDescription("""
Persists the order and replays the same totals when the same idempotency key is retried with the same payload.

Example usage:

```bash
curl -X POST 'http://localhost:5152/api/v1/orders/submit' \
  -H 'Content-Type: application/json' \
  -H 'Idempotency-Key: order-20260720-001' \
  -H 'X-Correlation-ID: corr-20260720-001' \
  -d '{
    "lineItems": [
      {
        "productId": "11111111-1111-1111-1111-111111111111",
        "quantity": 2,
        "productVersion": 1
      }
    ],
    "discount": {
      "type": "Percentage",
      "value": 10
    }
  }'
```
""")
            .Accepts<CalculateOrderRequest>("application/json")
            .Produces<CalculateOrderResponse>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict);
    }
}