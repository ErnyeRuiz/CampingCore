using CampingCore.Domain.Primitives;
using MediatR;

namespace CampingCore.Domain.Events;

public sealed record PasswordResetRequestedDomainEvent(
    Guid EventId,
    DateTime OccurredOn,
    int UserId,
    string Email,
    string Name,
    string PlainCode
) : IDomainEvent, INotification;
