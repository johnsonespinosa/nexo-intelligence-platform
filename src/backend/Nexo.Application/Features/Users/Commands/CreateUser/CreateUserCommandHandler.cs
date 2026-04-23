using MediatR;
using Nexo.Application.Common.DTOs;
using Nexo.Application.Common.Interfaces;
using Nexo.Domain.Entities;
using Nexo.Domain.Enums;

namespace Nexo.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public CreateUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<UserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
        {
            throw new InvalidOperationException($"User with email {request.Email} already exists");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            FullName = request.FullName,
            Role = Enum.Parse<UserRole>(request.Role, true),
            Tier = Enum.Parse<UserTier>(request.Tier, true),
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _userRepository.CreateAsync(user, cancellationToken);

        return new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role.ToString(),
            Tier = user.Tier.ToString(),
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };
    }
}
