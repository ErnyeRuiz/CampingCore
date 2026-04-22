namespace CampingCore.Application.Users.Commands.Login;

public record LoginResponse(int UserId, string Name, string Email, string Token);
