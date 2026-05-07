namespace CampingCore.Application.Users.Queries.GetUserById;

public record UserResponse(int Id, string Name, string Email, DateTime CreatedAt, string? RoleName);
