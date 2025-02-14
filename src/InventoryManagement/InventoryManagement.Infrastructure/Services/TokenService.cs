using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using InventoryManagement.Application.Configuration;
using InventoryManagement.Application.Services;
using InventoryManagement.Infrastructure.Tools;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace InventoryManagement.Infrastructure.Services;

public class TokenService(IOptions<JWTOptions> jwtOptions) : ITokenService
{
    public string GenerateToken(Guid userId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityKey = SecurityTools.GetSecurityKey(jwtOptions.Value.Key);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Audience = jwtOptions.Value.Audience,
            Issuer = jwtOptions.Value.Issuer,
            Subject = new ClaimsIdentity([new Claim("id", userId.ToString())]),
            Expires = DateTime.UtcNow.AddDays(31),
            SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}