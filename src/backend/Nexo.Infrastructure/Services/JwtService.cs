using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Nexo.Application.Common.Interfaces;

namespace Nexo.Infrastructure.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;
    private static readonly Dictionary<Guid, List<string>> _refreshTokens = new();

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateAccessToken(Guid userId, string email, string role)
    {
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(ClaimTypes.Role, role),
            new("userId", userId.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:AccessTokenExpirationMinutes"] ?? "60")),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!);

        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out _);

            return principal;
        }
        catch
        {
            return null;
        }
    }

    public bool ValidateRefreshToken(string refreshToken)
    {
        // Validación básica de formato
        return !string.IsNullOrEmpty(refreshToken) && refreshToken.Length >= 32;
    }

    public void StoreRefreshToken(Guid userId, string refreshToken)
    {
        if (!_refreshTokens.ContainsKey(userId))
        {
            _refreshTokens[userId] = new List<string>();
        }

        _refreshTokens[userId].Add(refreshToken);

        // Limitar a 5 tokens por usuario
        if (_refreshTokens[userId].Count > 5)
        {
            _refreshTokens[userId].RemoveAt(0);
        }
    }

    public bool ConsumeRefreshToken(Guid userId, string refreshToken)
    {
        if (_refreshTokens.TryGetValue(userId, out var tokens))
        {
            if (tokens.Contains(refreshToken))
            {
                tokens.Remove(refreshToken);
                return true;
            }
        }

        return false;
    }
}