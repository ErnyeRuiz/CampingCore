using CampingCore.Application.Abstractions.Messaging;

namespace CampingCore.Application.Roles.Commands.UpdateRole;

public record UpdateRoleCommand(int Id, string Name, string? Description) : ICommand;
