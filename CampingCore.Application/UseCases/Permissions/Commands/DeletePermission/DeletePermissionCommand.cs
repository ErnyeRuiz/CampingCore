using CampingCore.Application.Abstractions.Messaging;

namespace CampingCore.Application.Permissions.Commands.DeletePermission;

public record DeletePermissionCommand(int Id) : ICommand;
