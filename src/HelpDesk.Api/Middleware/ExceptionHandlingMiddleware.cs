using System.Net;
using System.Text.Json;
using HelpDesk.Api.Contracts.Common;
using HelpDesk.Application.Constants;
using HelpDesk.Application.Exceptions;
using HelpDesk.Domain.Exceptions;

namespace HelpDesk.Api.Middleware;

public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            if (context.Response.HasStarted)
            {
                logger.LogError(
                    exception,
                    "Cannot handle exception because response has already started.");

                throw;
            }

            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(
        HttpContext context, 
        Exception exception)
    {
        var (statusCode, errorCode, message) = MapException(exception);
        
        LogException(exception, statusCode);
        
        context.Response.Clear();
        context.Response.StatusCode = statusCode;

        var response = new ApiErrorResponse(
            Success: false,
            Data: null,
            Errors:
            [
                new ApiError(errorCode, message)
            ]);

        await context.Response.WriteAsJsonAsync(response);
    }

    private void LogException(Exception exception, int statusCode)
    {
        if (statusCode >= 500)
        {
            logger.LogError(exception,
                "Unhandled exception occurred");
        }
        else
        {
            logger.LogWarning(exception,
                "Request failed: {ErrorType}, {Message}", 
                exception.GetType().Name,
                exception.Message);
        }
    }

    private static (int statusCode, string errorCode, string message)
        MapException(Exception exception)
    {
        return exception switch
        {
            ConflictException conflict => (
                StatusCodes.Status409Conflict,
                conflict.Code,
                conflict.Message),

            DomainException domain=> (
                StatusCodes.Status400BadRequest,
                ErrorCodes.DomainError,
                domain.Message),

            _ => ((int)HttpStatusCode.InternalServerError,
                ErrorCodes.InternalServerError,
                "An unexpected error occurred")
        };
    }
}