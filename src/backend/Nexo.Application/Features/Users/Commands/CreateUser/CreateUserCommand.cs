using MediatR;
using Nexo.Application.Common.DTOs;

namespace Nexo.Application.Features.Users.Commands.CreateUser;

public record CreateUserCommand(string Email, string Password, string FullName, string Role, string Tier)
    : IRequest<UserResponse>;
