using MediatR;
using Nexo.Application.Common.DTOs;

namespace Nexo.Application.Features.Users.Queries.GetUserById;

public record GetUserByIdQuery(Guid Id) : IRequest<UserResponse?>;
