using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Domain.Common;
using CampingCore.Domain.ReadModels;
using CampingCore.Domain.Repositories;

namespace CampingCore.Application.Users.Queries.GetSystemUsersList;

internal sealed class GetSystemUsersListQueryHandler : IQueryHandler<GetSystemUsersListQuery, IReadOnlyList<UserSystemListItem>>
{
    private readonly IUserRepository _userRepository;

    public GetSystemUsersListQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<IReadOnlyList<UserSystemListItem>>> Handle(
        GetSystemUsersListQuery request,
        CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllWithStatisticsAsync(cancellationToken);
        return Result.Success(users);
    }
}
