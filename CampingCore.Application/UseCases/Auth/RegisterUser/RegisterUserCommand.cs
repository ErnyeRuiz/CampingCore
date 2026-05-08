using CampingCore.Application.Abstractions.Messaging;

namespace CampingCore.Application.Users.Commands.RegisterUser;

public record RegisterUserCommand(string Name, string Email, string Password, string Role) : ICommand<int>;
