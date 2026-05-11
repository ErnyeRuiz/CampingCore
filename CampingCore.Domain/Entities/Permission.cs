using CampingCore.Domain.Common;
using CampingCore.Domain.Primitives;

namespace CampingCore.Domain.Entities;

public class Permission : Entity<int>
{
    public static class Errors
    {
        public static readonly Error NameRequired = Error.Validation("Permission.NameRequired", "El nombre del permiso es obligatorio.");
        public static readonly Error NameTooLong  = Error.Validation("Permission.NameTooLong",  "El nombre del permiso no puede superar 100 caracteres.");
        public static readonly Error NotFound     = Error.NotFound("Permission.NotFound",        "El permiso no fue encontrado.");
    }

    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();

    protected Permission() : base(0) { }

    private Permission(string name, string? description) : base(0)
    {
        Name        = name;
        Description = description;
        CreatedAt   = DateTime.UtcNow;
    }

    public static Result<Permission> Create(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name)) return Result.Failure<Permission>(Errors.NameRequired);
        if (name.Length > 100)               return Result.Failure<Permission>(Errors.NameTooLong);

        return new Permission(name, description);
    }

    public Result Update(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name)) return Result.Failure(Errors.NameRequired);
        if (name.Length > 100)               return Result.Failure(Errors.NameTooLong);

        Name        = name;
        Description = description;

        return Result.Success();
    }
}
