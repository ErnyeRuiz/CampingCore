using CampingCore.Domain.Primitives;
using MediatR;

namespace CampingCore.Domain.Events;

public sealed record UserRegisteredDomainEvent(
    Guid EventId,
    DateTime OccurredOn,
    int UserId,
    string Email,
    string Name,
    string Code
) : IDomainEvent, INotification;
