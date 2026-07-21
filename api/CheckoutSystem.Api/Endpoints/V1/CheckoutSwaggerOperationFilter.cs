using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json.Nodes;

namespace CheckoutSystem.Api.Endpoints.V1;

internal sealed class CheckoutSwaggerOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        switch (operation.OperationId)
        {
            case "GetCatalogue":
                ApplyCatalogueExamples(operation);
                break;
            case "CalculateOrderTotals":
                ApplyRequestExample(operation);
                ApplySuccessResponseExample(operation);
                ApplyValidationProblemExample(operation);
                ApplyConcurrencyProblemExample(operation);
                break;
            case "SubmitOrder":
                ApplySubmitHeaders(operation);
                ApplyRequestExample(operation);
                ApplySuccessResponseExample(operation);
                ApplyMissingIdempotencyProblemExample(operation);
                ApplyConcurrencyProblemExample(operation);
                ApplyIdempotencyConflictProblemExample(operation);
                break;
        }
    }

    private static void ApplyCatalogueExamples(OpenApiOperation operation)
    {
        if (TryGetResponseContent(operation, "200", out var content))
        {
            content.Example = ParseJsonNode("""
[
  {
    "id": "11111111-1111-1111-1111-111111111111",
    "name": "Hot Coffee",
    "unitPrice": 3.5,
    "isTaxable": true,
    "version": 1
  },
  {
    "id": "22222222-2222-2222-2222-222222222222",
    "name": "Cold Sandwich",
    "unitPrice": 5.25,
    "isTaxable": false,
    "version": 1
  }
]
""");
        }
    }

    private static void ApplySubmitHeaders(OpenApiOperation operation)
    {
        operation.Parameters ??= [];

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "Idempotency-Key",
            In = ParameterLocation.Header,
            Required = true,
            Description = "Required key used to safely retry the same order submission.",
            Example = JsonValue.Create("order-20260720-001"),
            Schema = new OpenApiSchema { Type = JsonSchemaType.String },
        });

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "X-Correlation-ID",
            In = ParameterLocation.Header,
            Required = false,
            Description = "Optional correlation id for tracing a request across logs and retries.",
            Example = JsonValue.Create("corr-20260720-001"),
            Schema = new OpenApiSchema { Type = JsonSchemaType.String },
        });
    }

    private static void ApplyRequestExample(OpenApiOperation operation)
    {
        if (operation.RequestBody?.Content is { } requestBodyContent
            && requestBodyContent.TryGetValue("application/json", out var content)
            && content is not null)
        {
            content.Example = ParseJsonNode("""
{
  "lineItems": [
    {
      "productId": "11111111-1111-1111-1111-111111111111",
      "quantity": 2,
      "productVersion": 1
    },
    {
      "productId": "22222222-2222-2222-2222-222222222222",
      "quantity": 1,
      "productVersion": 1
    }
  ],
  "discount": {
    "type": "Percentage",
    "value": 10
  }
}
""");
        }
    }

    private static void ApplySuccessResponseExample(OpenApiOperation operation)
    {
        if (TryGetResponseContent(operation, "200", out var content))
        {
            content.Example = ParseJsonNode("""
{
  "subtotal": 12.25,
  "discountApplied": 1.23,
  "tax": 1.26,
  "total": 12.29,
  "splitShares": [
    {
      "payer": 1,
      "amount": 4.11
    },
    {
      "payer": 2,
      "amount": 4.09
    },
    {
      "payer": 3,
      "amount": 4.09
    }
  ]
}
""");
        }
    }

    private static void ApplyValidationProblemExample(OpenApiOperation operation)
    {
        if (TryGetResponseContent(operation, "400", out var content))
        {
            content.Example = ParseJsonNode("""
{
  "title": "Validation failed.",
  "detail": "One or more validation errors occurred.",
  "status": 400,
  "errors": [
    {
      "code": "quantity.invalid",
      "field": "lineItems[0].quantity",
      "message": "Quantity must be greater than zero."
    }
  ]
}
""");
        }
    }

    private static void ApplyMissingIdempotencyProblemExample(OpenApiOperation operation)
    {
        if (TryGetResponseContent(operation, "400", out var content))
        {
            content.Example = ParseJsonNode("""
{
  "title": "Validation failed.",
  "detail": "Idempotency-Key header is required for order submission.",
  "status": 400
}
""");
        }
    }

    private static void ApplyConcurrencyProblemExample(OpenApiOperation operation)
    {
        if (TryGetResponseContent(operation, "409", out var content))
        {
            content.Example = ParseJsonNode("""
{
  "title": "Concurrency conflict.",
  "detail": "Product version mismatch for product 11111111-1111-1111-1111-111111111111.",
  "status": 409
}
""");
        }
    }

    private static void ApplyIdempotencyConflictProblemExample(OpenApiOperation operation)
    {
        if (TryGetResponseContent(operation, "409", out var content))
        {
            content.Example = ParseJsonNode("""
{
  "title": "Idempotency conflict.",
  "detail": "Idempotency-Key was already used with a different request payload.",
  "status": 409
}
""");
        }
    }

    private static JsonNode ParseJsonNode(string json)
    {
        return JsonNode.Parse(json) ?? throw new InvalidOperationException("Swagger example JSON must be valid.");
    }

    private static bool TryGetResponseContent(OpenApiOperation operation, string statusCode, out OpenApiMediaType content)
    {
      content = default!;

      if (!operation.Responses.TryGetValue(statusCode, out var response)
        || response is null)
        {
            return false;
        }

      var responseContent = response.Content;

      if (responseContent is null
        || !responseContent.TryGetValue("application/json", out var mediaType)
        || mediaType is null)
      {
        return false;
      }

      content = mediaType;

        return true;
    }
}