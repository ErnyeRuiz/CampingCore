using CampingCore.Domain.Common;
using CampingCore.Domain.Events;
using CampingCore.Domain.Primitives;

namespace CampingCore.Domain.Entities;

public class User : AggregateRoot<int>
{
    public static class Errors
    {
        public static readonly Error NameRequired  = Error.Validation("User.NameRequired",  "El nombre es obligatorio.");
        public static readonly Error NameTooLong   = Error.Validation("User.NameTooLong",   "El nombre no puede superar 100 caracteres.");
        public static readonly Error EmailRequired = Error.Validation("User.EmailRequired", "El email es obligatorio.");
        public static readonly Error EmailTooLong  = Error.Validation("User.EmailTooLong",  "El email no puede superar 255 caracteres.");
        public static readonly Error PasswordHashRequired = Error.Validation("User.PasswordHashRequired", "El hash de contraseña es obligatorio.");
        public static readonly Error EmailAlreadyVerified = Error.Validation("User.EmailAlreadyVerified", "El email ya fue verificado.");
        public static readonly Error InvalidOrExpiredVerificationCode = Error.Validation("User.InvalidOrExpiredCode", "El código de verificación es inválido o ha expirado.");
    }

    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public int RoleId { get; private set; }
    public bool IsEmailVerified { get; private set; }
    public string? EmailVerificationCode { get; private set; }
    public DateTime? EmailVerificationCodeExpiresAt { get; private set; }

    public Role? Role { get; private set; }
    public ICollection<CampSite> CreatedCampSites { get; private set; } = new List<CampSite>();
    public ICollection<Review> Reviews { get; private set; } = new List<Review>();
    public ICollection<Favorite> Favorites { get; private set; } = new List<Favorite>();
    public ICollection<Trip> Trips { get; private set; } = new List<Trip>();

    protected User() : base(0) { }

    private User(string name, string email, string passwordHash, int roleId) : base(0)
    {
        Name         = name;
        Email        = email;
        PasswordHash = passwordHash;
        RoleId       = roleId;
        CreatedAt    = DateTime.UtcNow;
    }

    public static Result<User> Create(string name, string email, string passwordHash, int roleId)
    {
        if (string.IsNullOrWhiteSpace(name))         return Result.Failure<User>(Errors.NameRequired);
        if (name.Length > 100)                        return Result.Failure<User>(Errors.NameTooLong);
        if (string.IsNullOrWhiteSpace(email))         return Result.Failure<User>(Errors.EmailRequired);
        if (email.Length > 255)                       return Result.Failure<User>(Errors.EmailTooLong);
        if (string.IsNullOrWhiteSpace(passwordHash))  return Result.Failure<User>(Errors.PasswordHashRequired);
        if (roleId <= 0)                              return Result.Failure<User>(Error.Validation("User.InvalidRoleId", "El identificador de rol debe ser mayor a 0."));

        return new User(name, email, passwordHash, roleId);
    }

    public void GenerateEmailVerificationCode(string code)
    {
        EmailVerificationCode = BCrypt.Net.BCrypt.HashPassword(code);
        EmailVerificationCodeExpiresAt = DateTime.UtcNow.AddMinutes(15);

        RaiseDomainEvent(new UserRegisteredDomainEvent(
            Guid.NewGuid(),
            DateTime.UtcNow,
            Id,
            Email,
            Name,
            code));
    }

    public Result VerifyEmail(string code)
    {
        if (IsEmailVerified)
            return Result.Failure(Errors.EmailAlreadyVerified);

        if (string.IsNullOrEmpty(EmailVerificationCode)
            || EmailVerificationCodeExpiresAt is null
            || DateTime.UtcNow > EmailVerificationCodeExpiresAt.Value)
            return Result.Failure(Errors.InvalidOrExpiredVerificationCode);

        if (!BCrypt.Net.BCrypt.Verify(code, EmailVerificationCode))
            return Result.Failure(Errors.InvalidOrExpiredVerificationCode);

        IsEmailVerified = true;
        EmailVerificationCode = null;
        EmailVerificationCodeExpiresAt = null;

        return Result.Success();
    }

    public Result UpdateProfile(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return Result.Failure(Errors.NameRequired);
        if (name.Length > 100)               return Result.Failure(Errors.NameTooLong);

        Name = name;

        return Result.Success();
    }

    /// <summary>
    /// Actualiza datos del usuario por un administrador. Si <paramref name="passwordHash"/> es nulo o vacío, no se modifica la contraseña.
    /// </summary>
    public Result UpdateByAdmin(string name, string email, int roleId, string? passwordHash)
    {
        if (string.IsNullOrWhiteSpace(name))        return Result.Failure(Errors.NameRequired);
        if (name.Length > 100)                       return Result.Failure(Errors.NameTooLong);
        if (string.IsNullOrWhiteSpace(email))       return Result.Failure(Errors.EmailRequired);
        if (email.Length > 255)                      return Result.Failure(Errors.EmailTooLong);
        Name   = name;
        Email  = email.Trim();
        RoleId = roleId;

        if (!string.IsNullOrWhiteSpace(passwordHash))
            PasswordHash = passwordHash;

        return Result.Success();
    }
}
