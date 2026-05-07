using CampingCore.Application.Abstractions.Messaging;

namespace CampingCore.Application.Users.Commands.Login;

public record LoginCommand(string Email, string Password) : ICommand<LoginResponse>;
