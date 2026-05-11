using CampingCore.Application.Abstractions.Messaging;

namespace CampingCore.Application.Users.Commands.ResetPassword;

public record ResetPasswordCommand(string Email, string Code, string NewPassword) : ICommand;
