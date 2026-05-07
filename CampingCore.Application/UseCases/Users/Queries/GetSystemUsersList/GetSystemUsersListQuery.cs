using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Domain.ReadModels;

namespace CampingCore.Application.Users.Queries.GetSystemUsersList;

public record GetSystemUsersListQuery : IQuery<IReadOnlyList<UserSystemListItem>>;
