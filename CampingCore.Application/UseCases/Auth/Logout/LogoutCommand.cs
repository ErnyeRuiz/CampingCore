using CampingCore.Application.Abstractions.Messaging;

namespace CampingCore.Application.UseCases.Auth.Logout;

public sealed record LogoutCommand(string RefreshToken) : ICommand;
