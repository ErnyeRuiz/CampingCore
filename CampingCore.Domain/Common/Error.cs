namespace CampingCore.Domain.Common;

public sealed record Error(string Code, string Description)
{
    public static readonly Error None = new(string.Empty, string.Empty);
    public static readonly Error NullValue = new("Error.NullValue", "El valor nulo fue proporcionado.");

    public static Error NotFound(string entity, object id)
        => new($"{entity}.NotFound", $"{entity} con id '{id}' no fue encontrado.");

    public static Error Validation(string code, string description)
        => new(code, description);
}
