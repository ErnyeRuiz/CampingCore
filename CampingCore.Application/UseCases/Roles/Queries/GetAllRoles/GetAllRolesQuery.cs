using CampingCore.Application.Abstractions.Messaging;

namespace CampingCore.Application.Roles.Queries.GetAllRoles;

public record GetAllRolesQuery : IQuery<IReadOnlyList<RoleResponse>>;
