using CampingCore.Domain.Common;

namespace CampingCore.Domain.Repositories;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Ejecuta <paramref name="action"/> dentro de una transacción. Hace commit solo si el resultado es exitoso;
    /// en caso contrario revierte los cambios persistidos en la transacción.
    /// </summary>
    Task<Result<T>> ExecuteTransactionalAsync<T>(
        Func<Task<Result<T>>> action,
        CancellationToken cancellationToken = default);
}
