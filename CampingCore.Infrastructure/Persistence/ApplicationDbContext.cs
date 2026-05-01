using CampingCore.Domain.Entities;
using CampingCore.Domain.Primitives;
using CampingCore.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CampingCore.Infrastructure.Persistence;

public sealed class ApplicationDbContext : DbContext, IUnitOfWork
{
    private readonly IPublisher _publisher;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IPublisher publisher)
        : base(options)
    {
        _publisher = publisher;
    }

    public DbSet<User>           Users           { get; set; }
    public DbSet<Role>           Roles           { get; set; }
    public DbSet<Permission>     Permissions     { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<CampSite>       CampSites       { get; set; }
    public DbSet<CampSiteImage>  CampSiteImages  { get; set; }
    public DbSet<Review>         Reviews         { get; set; }
    public DbSet<Favorite>       Favorites       { get; set; }
    public DbSet<Trip>           Trips           { get; set; }
    public DbSet<TripCampSite>   TripCampSites   { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Carga automáticamente todas las clases IEntityTypeConfiguration<T> del ensamblado
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Recolecta los domain events de las entidades AggregateRoot antes de persistir,
        // de modo que los eventos se publiquen después del commit exitoso.
        var domainEvents = ChangeTracker.Entries<AggregateRoot>()
            .Select(e => e.Entity)
            .Where(e => e.GetDomainEvents().Count != 0)
            .SelectMany(e =>
            {
                var events = e.GetDomainEvents();
                e.ClearDomainEvents();
                return events; 
            })
            .ToList();

        var result = await base.SaveChangesAsync(cancellationToken);

        // Publica los eventos de dominio
        foreach (var domainEvent in domainEvents)
        {
            if (domainEvent is INotification notification)
                await _publisher.Publish(notification, cancellationToken);
        }

        return result;
    }
}
