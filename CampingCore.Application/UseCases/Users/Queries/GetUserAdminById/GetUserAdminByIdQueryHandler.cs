using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Domain.Common;
using CampingCore.Domain.Entities;
using CampingCore.Domain.Repositories;

namespace CampingCore.Application.Users.Queries.GetUserAdminById;

internal sealed class GetUserAdminByIdQueryHandler : IQueryHandler<GetUserAdminByIdQuery, UserAdminDetailResponse>
{
    private readonly IUserRepository _userRepository;

    public GetUserAdminByIdQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<UserAdminDetailResponse>> Handle(GetUserAdminByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdWithRoleAsync(request.UserId, cancellationToken);

        if (user is null)
            return Result.Failure<UserAdminDetailResponse>(Error.NotFound(nameof(User), request.UserId));

        return new UserAdminDetailResponse(
            user.Id,
            user.Name,
            user.Email,
            user.CreatedAt,
            user.RoleId,
            user.Role?.Name);
    }
}
