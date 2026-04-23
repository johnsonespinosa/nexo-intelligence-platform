using MediatR;
using Nexo.Application.Common.DTOs;

namespace Nexo.Application.Features.Users.Commands.UpdateUser;

public record UpdateUserCommand(
    Guid Id,
    string? Email,
    string? FullName,
    string? Role,
    string? Tier,
    bool? IsActive) : IRequest<UserResponse?>;
