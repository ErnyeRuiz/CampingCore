using CampingCore.Application.Abstractions.Messaging;

namespace CampingCore.Application.Permissions.Queries.GetPermissionById;

public record GetPermissionByIdQuery(int Id) : IQuery<PermissionResponse>;
