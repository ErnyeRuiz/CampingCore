using CampingCore.Domain.Common;
using CampingCore.Domain.Entities;
using CampingCore.Domain.Primitives;
using CampingCore.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

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
    public DbSet<RefreshToken>   RefreshTokens   { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Carga automáticamente todas las clases IEntityTypeConfiguration<T> del ensamblado
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public async Task<Result<T>> ExecuteTransactionalAsync<T>(
        Func<Task<Result<T>>> action,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var result = await action();
            if (result.IsFailure)
            {
                await transaction.RollbackAsync(cancellationToken);
                return result;
            }

            await transaction.CommitAsync(cancellationToken);
            return result;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var aggregates = ChangeTracker.Entries<AggregateRoot<int>>()
            .Select(e => e.Entity)
            .Where(e => e.GetDomainEvents().Count != 0)
            .ToList();

        var domainEvents = aggregates
            .SelectMany(e => e.GetDomainEvents())
            .ToList();

        aggregates.ForEach(e => e.ClearDomainEvents());

        var result = await base.SaveChangesAsync(cancellationToken);

        foreach (var domainEvent in domainEvents)
        {
            if (domainEvent is INotification notification)
                await _publisher.Publish(notification, cancellationToken);
        }

        return result;
    }
}
