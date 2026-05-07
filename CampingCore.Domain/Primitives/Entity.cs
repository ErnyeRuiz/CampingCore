using System.Collections.Generic;

namespace CampingCore.Domain.Primitives;

public abstract class Entity<TId> : IEquatable<Entity<TId>>
{
    protected Entity(TId id)
    {
        Id = id;
    }

    public TId Id { get; private set; } = default!;

    public static bool operator ==(Entity<TId>? first, Entity<TId>? second)
        => first is not null && second is not null && first.Equals(second);

    public static bool operator !=(Entity<TId>? first, Entity<TId>? second)
        => !(first == second);

    public bool Equals(Entity<TId>? other)
    {
        if (other is null) return false;
        if (other.GetType() != GetType()) return false;
        return EqualityComparer<TId>.Default.Equals(other.Id, Id);
    }

    public override bool Equals(object? obj)
        => obj is Entity<TId> entity && Equals(entity);

    public override int GetHashCode()
        => Id is null ? 0 : EqualityComparer<TId>.Default.GetHashCode(Id);
}
