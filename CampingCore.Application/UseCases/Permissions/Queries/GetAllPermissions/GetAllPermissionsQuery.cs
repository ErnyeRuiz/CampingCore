using CampingCore.Application.Abstractions.Messaging;

namespace CampingCore.Application.Permissions.Queries.GetAllPermissions;

public record GetAllPermissionsQuery : IQuery<IReadOnlyList<PermissionResponse>>;
