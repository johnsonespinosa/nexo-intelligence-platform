using MediatR;
using Nexo.Application.Common.DTOs;
using Nexo.Application.Common.Interfaces;
using Nexo.Domain.Enums;

namespace Nexo.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserResponse?>
{
    private readonly IUserRepository _userRepository;

    public UpdateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserResponse?> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

        if (user is null) return null;

        if (request.Email is not null) user.Email = request.Email;
        if (request.FullName is not null) user.FullName = request.FullName;
        if (request.Role is not null) user.Role = Enum.Parse<UserRole>(request.Role, true);
        if (request.Tier is not null) user.Tier = Enum.Parse<UserTier>(request.Tier, true);
        if (request.IsActive.HasValue) user.IsActive = request.IsActive.Value;

        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user, cancellationToken);

        return new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role.ToString(),
            Tier = user.Tier.ToString(),
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt
        };
    }
}
