using MediatR;
using Nexo.Application.Common.DTOs;

namespace Nexo.Application.Features.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<LoginResponse>;
