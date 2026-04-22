using Microsoft.EntityFrameworkCore;

namespace CampingCore.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repositorio genérico base. TEntity es la entidad de dominio, TId es el tipo de su PK (int, Guid, etc.).
/// Al usar FindAsync EF Core localiza la entidad primero en el ChangeTracker (memoria) antes de ir a la BD,
/// lo que evita queries duplicadas en el mismo request.
/// </summary>
public abstract class Repository<TEntity, TId>
    where TEntity : class
{
    protected readonly ApplicationDbContext Context;

    protected Repository(ApplicationDbContext context) => Context = context;

    public async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
        => await Context.Set<TEntity>().FindAsync([id], cancellationToken);

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        => await Context.Set<TEntity>().ToListAsync(cancellationToken);

    public void Add(TEntity entity)    => Context.Set<TEntity>().Add(entity);
    public void Update(TEntity entity) => Context.Set<TEntity>().Update(entity);
    public void Remove(TEntity entity) => Context.Set<TEntity>().Remove(entity);
}
