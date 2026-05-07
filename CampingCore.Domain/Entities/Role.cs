using CampingCore.Domain.Common;
using CampingCore.Domain.Primitives;

namespace CampingCore.Domain.Entities;

public class Role : Entity<int>
{
    public static class Errors
    {
        public static readonly Error NameRequired = Error.Validation("Role.NameRequired", "El nombre del rol es obligatorio.");
        public static readonly Error NameTooLong  = Error.Validation("Role.NameTooLong",  "El nombre del rol no puede superar 100 caracteres.");
        public static readonly Error NotFound     = Error.NotFound("Role.NotFound",        "El rol no fue encontrado.");
    }

    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();

    protected Role() : base(0) { }

    private Role(string name, string? description) : base(0)
    {
        Name        = name;
        Description = description;
        CreatedAt   = DateTime.UtcNow;
    }

    public static Result<Role> Create(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name)) return Result.Failure<Role>(Errors.NameRequired);
        if (name.Length > 100)               return Result.Failure<Role>(Errors.NameTooLong);

        return new Role(name, description);
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
