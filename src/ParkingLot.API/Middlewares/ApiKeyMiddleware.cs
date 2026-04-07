using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace ParkingLot.API.Middlewares;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private const string ApiKeyHeaderName = "X-Api-Key";

    public ApiKeyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IConfiguration configuration)
    {
        // Skip API Key auth for standard auth endpoints to not double-block
        if (context.Request.Path.StartsWithSegments("/api/v1/auth"))
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("API Key is missing.");
            return;
        }

        var configuredApiKey = configuration.GetValue<string>("ApiKeySettings:GlobalKey");

        if (!string.Equals(extractedApiKey, configuredApiKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized client.");
            return;
        }

        await _next(context);
    }
}
