using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Domain.Exceptions;

namespace OrderManagement.Api.ExceptionHandling;

public sealed class GlobalExceptionHandler
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        ProblemDetails problemDetails =
            new ProblemDetails();

        switch (exception)
        {
            case ValidationException validationException:
                problemDetails.Status = StatusCodes.Status400BadRequest;

                problemDetails.Title = "Validation failed.";

                problemDetails.Extensions["errors"] =
                    validationException.Errors
                        .Select(error => new
                        {
                            error.PropertyName,
                            error.ErrorMessage
                        })
                        .ToArray();

                break;

            case DomainException:
                problemDetails.Status =
                    StatusCodes.Status400BadRequest;

                problemDetails.Title =
                    "Business rule violation.";

                problemDetails.Detail =
                    exception.Message;

                break;

            case KeyNotFoundException:
                problemDetails.Status =
                    StatusCodes.Status404NotFound;

                problemDetails.Title =
                    "Resource not found.";

                problemDetails.Detail =
                    exception.Message;

                break;

            default:
                problemDetails.Status =
                    StatusCodes.Status500InternalServerError;

                problemDetails.Title =
                    "An unexpected error occurred.";

                break;
        }

        httpContext.Response.StatusCode =
            problemDetails.Status.Value;

        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken);

        return true;
    }
}