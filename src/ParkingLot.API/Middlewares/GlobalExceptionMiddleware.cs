using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ParkingLot.Domain.Exceptions;

namespace ParkingLot.API.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception has occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = HttpStatusCode.InternalServerError;
        var detail = "An unexpected error occurred.";

        switch (exception)
        {
            case SpotNotAvailableException _:
            case VehicleAlreadyParkedException _:
            case TicketAlreadyPaidException _:
                statusCode = HttpStatusCode.Conflict;
                detail = exception.Message;
                break;
            case TicketNotFoundException _:
                statusCode = HttpStatusCode.NotFound;
                detail = exception.Message;
                break;
            case InvalidTicketStateException _:
                statusCode = HttpStatusCode.BadRequest;
                detail = exception.Message;
                break;
        }

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = statusCode.ToString(),
            Detail = detail
        };

        var result = JsonSerializer.Serialize(problemDetails);
        return context.Response.WriteAsync(result);
    }
}
