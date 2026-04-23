using MediatR;
using Nexo.Application.Common.DTOs;
using Nexo.Application.Common.Interfaces;

namespace Nexo.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtService jwtService) : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IJwtService _jwtService = jwtService;

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        var accessToken = _jwtService.GenerateAccessToken(user.Id, user.Email, user.Role.ToString());
        var refreshToken = _jwtService.GenerateRefreshToken();

        return new LoginResponse(
            accessToken,
            refreshToken,
            3600, // 1 hour
            user.Id,
            user.Email,
            user.Role.ToString()
        );
    }
}
