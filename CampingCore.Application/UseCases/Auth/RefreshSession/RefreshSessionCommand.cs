using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Application.Users.Commands.Login;

namespace CampingCore.Application.UseCases.Auth.RefreshSession;

public sealed record RefreshSessionCommand(string RefreshToken) : ICommand<LoginResponse>;
