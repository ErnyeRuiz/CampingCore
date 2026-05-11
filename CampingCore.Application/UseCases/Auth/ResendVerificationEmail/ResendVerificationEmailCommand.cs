using CampingCore.Application.Abstractions.Messaging;

namespace CampingCore.Application.UseCases.Auth.ResendVerificationEmail;

public record ResendVerificationEmailCommand(string Email) : ICommand;
