using System.Net;
using InventoryManagement.Application.Errors;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Middlewares;

public class ExceptionToProblemDetailsHandler(ILogger<ExceptionToProblemDetailsHandler> logger) :
    IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        switch (exception)
        {
            case ProvidedDataIsInvalidError providedDataIsInvalidErrorException:
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Title = providedDataIsInvalidErrorException.Message,
                    Status = (int)HttpStatusCode.Forbidden
                }, cancellationToken);
                break;

            case AccessDeniedError accessDeniedErrorException:
                httpContext.Response.StatusCode = 403;
                await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Title = accessDeniedErrorException.Message,
                    Status = (int)HttpStatusCode.Forbidden
                }, cancellationToken);
                break;

            case NotFoundError notFoundErrorException:
                httpContext.Response.StatusCode = 404;
                await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Title = notFoundErrorException.Message,
                    Status = (int)HttpStatusCode.NotFound
                }, cancellationToken);
                break;

            default:
                logger.LogError(exception, "Unhandled exception during request occurred");

                httpContext.Response.StatusCode = 500;
                await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Title = "Unhandled exception during request occurred",
                    Status = (int)HttpStatusCode.InternalServerError
                }, cancellationToken);
                break;
        }

        return true;
    }
}