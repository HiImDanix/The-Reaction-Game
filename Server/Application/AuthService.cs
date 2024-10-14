using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application;

public class AuthService: IAuthService
{
    private const string SecurityAlgorithm = SecurityAlgorithms.HmacSha256;
    private const string PlayerNameClaimType = "playerName";
    private const string RoomIdClaimType = "roomId";
    
    private readonly string SecretKey;
    private readonly SymmetricSecurityKey _signingKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly TimeSpan _tokenLifetime;
    
    public AuthService(IConfiguration configuration)
    {
        SecretKey = configuration["JWT:SECRET_KEY"] ?? throw new ArgumentNullException("SECRET_KEY is missing");
        _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
        _issuer = configuration["JWT:ISSUER"] ?? throw new ArgumentNullException("ISSUER is missing");
        _audience = configuration["JWT:AUDIENCE"] ?? throw new ArgumentNullException("AUDIENCE is missing");
        _tokenLifetime = TimeSpan.FromHours(int.Parse(configuration["JWT:LIFETIME_HOURS"] ?? "24"));
    }
    
    public string GenerateToken(string playerId, string playerName, string roomId)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, playerId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64),
            new Claim(PlayerNameClaimType, playerName),
            new Claim(RoomIdClaimType, roomId),
        };
        
        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.Add(_tokenLifetime),
            signingCredentials: new SigningCredentials(_signingKey, SecurityAlgorithm)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string ExtractClaimFromToken(string token, string claimType)
    {
        if (string.IsNullOrEmpty(token))
            throw new ArgumentNullException(nameof(token));

        if (string.IsNullOrEmpty(claimType))
            throw new ArgumentNullException(nameof(claimType));

        try
        {
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            return jwtToken.Claims.First(claim => claim.Type == claimType).Value;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to extract claim {claimType} from token", ex);
        }
    }

    public string ExtractPlayerIdFromToken(string token) => ExtractClaimFromToken(token, JwtRegisteredClaimNames.Sub);

    public string ExtractRoomIdFromToken(string token) => ExtractClaimFromToken(token, "roomId");

    public string ExtractPlayerNameFromToken(string token) => ExtractClaimFromToken(token, "playerName");
}