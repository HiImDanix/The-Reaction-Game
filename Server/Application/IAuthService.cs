namespace Application;

public interface IAuthService
{
    string GenerateToken(string playerId, string playerName, string roomId); 
    bool ValidateToken(string sessionToken);
    string ExtractPlayerIdFromToken(string sessionToken);
    string ExtractRoomIdFromToken(string sessionToken);
    string ExtractPlayerNameFromToken(string sessionToken);
}