namespace CampingCore.Application.Users.Queries.GetUserAdminById;

public record UserAdminDetailResponse(
    int Id,
    string Name,
    string Email,
    DateTime CreatedAt,
    int RoleId,
    string? RoleName);
