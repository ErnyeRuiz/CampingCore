using CampingCore.Application.Abstractions.Messaging;

namespace CampingCore.Application.Roles.Commands.DeleteRole;

public record DeleteRoleCommand(int Id) : ICommand;
