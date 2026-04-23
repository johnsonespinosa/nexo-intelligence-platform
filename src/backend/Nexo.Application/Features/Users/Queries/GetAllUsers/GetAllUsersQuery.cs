using MediatR;
using Nexo.Application.Common.DTOs;

namespace Nexo.Application.Features.Users.Queries.GetAllUsers;

public record GetAllUsersQuery(int Page, int PageSize) : IRequest<PaginatedResponse<UserResponse>>;
