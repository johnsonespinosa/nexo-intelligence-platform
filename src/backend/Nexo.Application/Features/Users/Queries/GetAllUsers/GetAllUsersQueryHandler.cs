using MediatR;
using Nexo.Application.Common.DTOs;
using Nexo.Application.Common.Interfaces;

namespace Nexo.Application.Features.Users.Queries.GetAllUsers;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, PaginatedResponse<UserResponse>>
{
    private readonly IUserRepository _userRepository;

    public GetAllUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<PaginatedResponse<UserResponse>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllAsync(request.Page, request.PageSize, cancellationToken);
        var totalCount = await _userRepository.GetCountAsync(cancellationToken);

        return new PaginatedResponse<UserResponse>(
            users.Select(u => new UserResponse
            {
                Id = u.Id,
                Email = u.Email,
                FullName = u.FullName,
                Role = u.Role.ToString(),
                Tier = u.Tier.ToString(),
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                LastLoginAt = u.LastLoginAt
            }),
            totalCount,
            request.Page,
            request.PageSize
        );
    }
}
