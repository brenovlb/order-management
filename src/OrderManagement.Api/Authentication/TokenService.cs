using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace OrderManagement.Api.Authentication;

public sealed class TokenService
{
    private readonly JwtOptions _jwtOptions;

    public TokenService(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }

    public string GenerateToken(string email)
    {
        Claim[] claims =
        {
            new Claim(JwtRegisteredClaimNames.Sub, email),
            new Claim(JwtRegisteredClaimNames.Email, email)
        };

        SymmetricSecurityKey key =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtOptions.Key));

        SigningCredentials credentials =
            new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

        DateTime expiresAt =
            DateTime.UtcNow.AddMinutes(
                _jwtOptions.ExpirationMinutes);

        JwtSecurityToken token =
            new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials);

        JwtSecurityTokenHandler handler =
            new JwtSecurityTokenHandler();

        return handler.WriteToken(token);
    }
}