using CampingCore.Application.Abstractions.Messaging;

namespace CampingCore.Application.Roles.Commands.SetRolePermissions;

public record SetRolePermissionsCommand(int RoleId, IReadOnlyList<int> PermissionIds) : ICommand;
