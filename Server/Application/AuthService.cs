using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application;

public class AuthService: IAuthService
{
    private readonly IConfiguration _configuration;
    private string SecretKey;
    private readonly SymmetricSecurityKey _signingKey;
    
    public AuthService(IConfiguration configuration)
    {
        _configuration = configuration;
        SecretKey = _configuration["JWT:SECRET_KEY"] ?? throw new ArgumentNullException("SECRET_KEY is missing");
        _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
    }
    
    public string GenerateToken(string playerId, string playerName, string roomId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, playerId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim("playerName", playerName),
                new Claim("roomId", roomId),
            }),
            Expires = DateTime.UtcNow.AddHours(24),
            SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public bool ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out _);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public string ExtractPlayerIdFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
        return jwtToken.Claims.First(claim => claim.Type == JwtRegisteredClaimNames.Sub).Value;
    }

    public string ExtractRoomIdFromToken(string sessionToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(sessionToken);
        return jwtToken.Claims.First(claim => claim.Type == "roomId").Value;
    }

    public string ExtractPlayerNameFromToken(string sessionToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(sessionToken);
        return jwtToken.Claims.First(claim => claim.Type == "playerName").Value;
    }
}