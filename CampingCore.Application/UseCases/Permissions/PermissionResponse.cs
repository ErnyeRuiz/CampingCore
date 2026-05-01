namespace CampingCore.Application.Permissions;

public record PermissionResponse(int Id, string Name, string? Description, DateTime CreatedAt);
