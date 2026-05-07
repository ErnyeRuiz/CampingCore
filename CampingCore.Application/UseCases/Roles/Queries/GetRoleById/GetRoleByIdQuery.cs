using CampingCore.Application.Abstractions.Messaging;

namespace CampingCore.Application.Roles.Queries.GetRoleById;

public record GetRoleByIdQuery(int Id) : IQuery<RoleResponse>;
