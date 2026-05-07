using CampingCore.Common;
using Microsoft.AspNetCore.Diagnostics;

namespace CampingCore.Middleware;

/// <summary>
/// Captura cualquier excepción no controlada y devuelve un <c>500</c> con mensaje amigable,
/// registrando el detalle completo en el log para diagnóstico interno.
/// </summary>
internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Excepción no controlada en {Method} {Path}.",
            httpContext.Request.Method,
            httpContext.Request.Path);

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        await httpContext.Response.WriteAsJsonAsync(
            ApiResponse.Fail(500, "Ocurrió un error inesperado. Por favor intente más tarde."),
            cancellationToken);

        return true;
    }
}
