using CheckoutSystem.Api.Contracts.V1;
using CheckoutSystem.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace CheckoutSystem.Api.Middleware;

internal sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException exception)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            var problem = new ProblemDetails
            {
                Title = "Validation failed.",
                Detail = "One or more validation errors occurred.",
                Status = StatusCodes.Status400BadRequest,
            };

            problem.Extensions["errors"] = exception.Errors
                .Select(error => new ValidationErrorResponse(error.Code, error.Field, error.Message))
                .ToArray();

            await context.Response.WriteAsJsonAsync(problem);
        }
        catch (IdempotencyConflictException exception)
        {
            context.Response.StatusCode = StatusCodes.Status409Conflict;
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Title = "Idempotency conflict.",
                Detail = exception.Message,
                Status = StatusCodes.Status409Conflict,
            });
        }
        catch (ConcurrencyConflictException exception)
        {
            context.Response.StatusCode = StatusCodes.Status409Conflict;
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Title = "Concurrency conflict.",
                Detail = exception.Message,
                Status = StatusCodes.Status409Conflict,
            });
        }
        catch (NotFoundException exception)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Title = "Resource not found.",
                Detail = exception.Message,
                Status = StatusCodes.Status404NotFound,
            });
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception occurred.");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Title = "Unexpected error.",
                Detail = "An unexpected error occurred while processing the request.",
                Status = StatusCodes.Status500InternalServerError,
            });
        }
    }
}