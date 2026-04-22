using CampingCore.Domain.Common;

namespace CampingCore.Domain.Entities;

public class User
{
    public static class Errors
    {
        public static readonly Error NameRequired  = Error.Validation("User.NameRequired",  "El nombre es obligatorio.");
        public static readonly Error NameTooLong   = Error.Validation("User.NameTooLong",   "El nombre no puede superar 100 caracteres.");
        public static readonly Error EmailRequired = Error.Validation("User.EmailRequired", "El email es obligatorio.");
        public static readonly Error EmailTooLong  = Error.Validation("User.EmailTooLong",  "El email no puede superar 255 caracteres.");
        public static readonly Error PasswordHashRequired = Error.Validation("User.PasswordHashRequired", "El hash de contraseña es obligatorio.");
    }

    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    public ICollection<CampSite> CreatedCampSites { get; private set; } = new List<CampSite>();
    public ICollection<Review> Reviews { get; private set; } = new List<Review>();
    public ICollection<Favorite> Favorites { get; private set; } = new List<Favorite>();
    public ICollection<Trip> Trips { get; private set; } = new List<Trip>();

    protected User() { }

    private User(string name, string email, string passwordHash)
    {
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        CreatedAt = DateTime.UtcNow;
    }

    public static Result<User> Create(string name, string email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(name))         return Result.Failure<User>(Errors.NameRequired);
        if (name.Length > 100)                        return Result.Failure<User>(Errors.NameTooLong);
        if (string.IsNullOrWhiteSpace(email))         return Result.Failure<User>(Errors.EmailRequired);
        if (email.Length > 255)                       return Result.Failure<User>(Errors.EmailTooLong);
        if (string.IsNullOrWhiteSpace(passwordHash))  return Result.Failure<User>(Errors.PasswordHashRequired);

        return new User(name, email, passwordHash);
    }

    public Result UpdateProfile(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return Result.Failure(Errors.NameRequired);
        if (name.Length > 100)               return Result.Failure(Errors.NameTooLong);

        Name = name;

        return Result.Success();
    }
}
