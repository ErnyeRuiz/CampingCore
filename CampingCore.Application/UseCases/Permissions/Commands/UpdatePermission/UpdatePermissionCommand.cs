using CampingCore.Application.Abstractions.Messaging;

namespace CampingCore.Application.Permissions.Commands.UpdatePermission;

public record UpdatePermissionCommand(int Id, string Name, string? Description) : ICommand;
