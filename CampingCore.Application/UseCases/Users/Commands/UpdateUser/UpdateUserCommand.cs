using CampingCore.Application.Abstractions.Messaging;

namespace CampingCore.Application.Users.Commands.UpdateUser;

public record UpdateUserCommand(string Name, int RequestingUserId) : ICommand;
