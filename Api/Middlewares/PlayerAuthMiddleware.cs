using System.Text.Json;
using Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ReaktlyC.Middlewares;

public class PlayerAuthMiddleware
{
    private readonly RequestDelegate _next;

    public PlayerAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IAuthService authService)
    {
        var sessionToken = context.Request.Headers["Authorization"].FirstOrDefault();
        if (string.IsNullOrEmpty(sessionToken))
        {
            await SendUnauthorizedResponse(context, "Authorization header is missing");
            return;
        }

        if (!authService.ValidateToken(sessionToken))
        {
            await SendUnauthorizedResponse(context, "Player is not authorized");
            return;
        }

        context.Items["PlayerId"] = authService.ExtractPlayerIdFromToken(sessionToken);
        context.Items["RoomId"] = authService.ExtractRoomIdFromToken(sessionToken);
        context.Items["PlayerName"] = authService.ExtractPlayerNameFromToken(sessionToken);
        context.Items["IsAuthorized"] = true;

        await _next(context);
    }

    private static async Task SendUnauthorizedResponse(HttpContext context, string detail)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/problem+json";
        var problem = new ProblemDetails
        {
            Status = StatusCodes.Status401Unauthorized,
            Title = "Unauthorized",
            Detail = detail
        };
        await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
    }
}

public static class PlayerAuthMiddlewareExtensions
{
    public static IApplicationBuilder UsePlayerAuth(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<PlayerAuthMiddleware>();
    }
}

public static class HttpContextExtensions
{
    public static string GetPlayerId(this HttpContext context)
    {
        return context.Items["PlayerId"] as string 
            ?? throw new UnauthorizedAccessException("Player ID not found in context");
    }

    public static string GetRoomId(this HttpContext context)
    {
        return context.Items["RoomId"] as string 
            ?? throw new UnauthorizedAccessException("Room ID not found in context");
    }

    public static string GetPlayerName(this HttpContext context)
    {
        return context.Items["PlayerName"] as string 
            ?? throw new UnauthorizedAccessException("Player name not found in context");
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequirePlayerAuthAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.Items.ContainsKey("IsAuthorized"))
        {
            context.Result = new UnauthorizedObjectResult(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized",
                Detail = "Not an authorized player"
            });
        }
    }
}