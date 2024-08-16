using Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ReaktlyC.Attributes;

// [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
// public class RequirePlayerAuthAttribute : Attribute, IAuthorizationFilter
// {
//     public void OnAuthorization(AuthorizationFilterContext context)
//     {
//         // Get Authorization token from the request headers
//         var sessionToken = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
//         if (string.IsNullOrEmpty(sessionToken))
//         {
//             context.Result = new UnauthorizedResult();
//             return;
//         }
//         
//         // Validate the token
//         var authService = context.HttpContext.RequestServices.GetRequiredService<IAuthService>();
//         if (!authService.ValidateToken(sessionToken))
//         {
//             context.Result = new UnauthorizedResult();
//             return;
//         }
//         
//         // Put the player ID, room ID, and player name in the HTTP context
//         context.HttpContext.Items["PlayerId"] = authService.ExtractPlayerIdFromToken(sessionToken);
//         context.HttpContext.Items["RoomId"] = authService.ExtractRoomIdFromToken(sessionToken);
//         context.HttpContext.Items["PlayerName"] = authService.ExtractPlayerNameFromToken(sessionToken);
//         
//     }
// }
//
// public static class HttpContextExtensions
// {
//     public static string GetPlayerId(this HttpContext context)
//     {
//         return context.Items["PlayerId"] as string 
//                ?? throw new UnauthorizedAccessException("Player ID not found in context");
//     }
//
//     public static string GetRoomId(this HttpContext context)
//     {
//         return context.Items["RoomId"] as string 
//                ?? throw new UnauthorizedAccessException("Room ID not found in context");
//     }
//
//     public static string GetPlayerName(this HttpContext context)
//     {
//         return context.Items["PlayerName"] as string 
//                ?? throw new UnauthorizedAccessException("Player name not found in context");
//     }
// }