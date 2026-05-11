using CampingCore.Application.Abstractions.Messaging;

namespace CampingCore.Application.Users.Commands.ForgotPassword;

public record ForgotPasswordCommand(string Email) : ICommand;
