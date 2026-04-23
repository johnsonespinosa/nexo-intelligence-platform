using System.Security.Claims;

namespace Nexo.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(Guid userId, string email, string role);
    string GenerateRefreshToken();
    ClaimsPrincipal? ValidateToken(string token);
}