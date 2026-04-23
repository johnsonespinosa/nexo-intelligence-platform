namespace Nexo.Application.Common.DTOs;

public record LoginResponse(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn,
    Guid UserId,
    string Email,
    string Role);
