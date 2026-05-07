using CampingCore.Application.Abstractions.Messaging;

namespace CampingCore.Application.UseCases.Auth.VerifyEmail;

public record VerifyEmailCommand(int UserId, string Code) : ICommand;
