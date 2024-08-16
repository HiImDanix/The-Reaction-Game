namespace ReaktlyC.Extensions;

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