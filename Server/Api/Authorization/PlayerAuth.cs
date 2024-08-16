using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Application;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;

namespace ReaktlyC.Authorization;

public class PlayerAuthRequirement : IAuthorizationRequirement { }

public class PlayerAuthHandler : AuthorizationHandler<PlayerAuthRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAuthService _authService;

    public PlayerAuthHandler(IHttpContextAccessor httpContextAccessor, IAuthService authService)
    {
        _httpContextAccessor = httpContextAccessor;
        _authService = authService;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PlayerAuthRequirement requirement)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        
        if (!httpContext.User.Identity.IsAuthenticated)
        {
            context.Fail();
            return Task.CompletedTask;
        }
        
        string sessionToken = httpContext.Request.Headers[HeaderNames.Authorization];

        // If the Authorization header is empty, try to get the token from query string (SignalR requests)
        if (string.IsNullOrEmpty(sessionToken))
        {
            sessionToken = httpContext.Request.Query["access_token"];
        }

        if (!string.IsNullOrEmpty(sessionToken))
        {
            ProcessToken(httpContext, sessionToken);
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }

        context.Succeed(requirement);
        return Task.CompletedTask;
    }

    private void ProcessToken(HttpContext httpContext, string sessionToken)
    {
        var token = sessionToken.StartsWith("Bearer ") ? sessionToken.Substring(7) : sessionToken;
        if (!string.IsNullOrEmpty(token))
        {
            httpContext.Items["PlayerId"] = _authService.ExtractPlayerIdFromToken(token);
            httpContext.Items["RoomId"] = _authService.ExtractRoomIdFromToken(token);
            httpContext.Items["PlayerName"] = _authService.ExtractPlayerNameFromToken(token);
        }
    }
}