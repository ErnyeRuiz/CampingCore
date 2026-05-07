using CampingCore.Application.Abstractions.Messaging;

namespace CampingCore.Application.Permissions.Commands.CreatePermission;

public record CreatePermissionCommand(string Name, string? Description) : ICommand<int>;
