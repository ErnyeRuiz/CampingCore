namespace CampingCore.Common;

/// <summary>
/// Sobre uniforme de respuesta para todos los endpoints de la API.
/// </summary>
/// <typeparam name="T">Tipo del payload. Puede ser <c>null</c> en respuestas sin datos o en errores.</typeparam>
public sealed record ApiResponse<T>(bool Success, int Code, T? Data, string Message);

/// <summary>
/// Métodos de fábrica para construir instancias de <see cref="ApiResponse{T}"/>.
/// </summary>
public static class ApiResponse
{
    /// <summary>Respuesta exitosa con datos.</summary>
    public static ApiResponse<T> Success<T>(T data, int code = 200, string message = "Operación exitosa.")
        => new(true, code, data, message);

    /// <summary>Respuesta de error con <c>data</c> nulo.</summary>
    public static ApiResponse<object?> Fail(int code, string message)
        => new(false, code, null, message);
}
