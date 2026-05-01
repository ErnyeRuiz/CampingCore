using CampingCore.Application.Abstractions.Messaging;

namespace CampingCore.Application.Roles.Commands.CreateRole;

public record CreateRoleCommand(string Name, string? Description) : ICommand<int>;
