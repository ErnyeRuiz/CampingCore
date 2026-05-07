using CampingCore.Application.Permissions;

namespace CampingCore.Application.Roles;

public record RoleResponse(
    int Id,
    string Name,
    string? Description,
    DateTime CreatedAt,
    IReadOnlyList<PermissionResponse> Permissions);
