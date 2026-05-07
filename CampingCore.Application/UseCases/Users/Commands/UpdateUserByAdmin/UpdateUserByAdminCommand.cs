using CampingCore.Application.Abstractions.Messaging;

namespace CampingCore.Application.Users.Commands.UpdateUserByAdmin;

public record UpdateUserByAdminCommand(
    int UserId,
    string Name,
    string Email,
    int RoleId,
    string? Password) : ICommand;
