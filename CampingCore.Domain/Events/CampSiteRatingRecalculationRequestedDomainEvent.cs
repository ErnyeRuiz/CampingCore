using CampingCore.Domain.Primitives;
using MediatR;

namespace CampingCore.Domain.Events;

public sealed record CampSiteRatingRecalculationRequestedDomainEvent(
    Guid EventId,
    DateTime OccurredOn,
    int CampSiteId
) : IDomainEvent, INotification;
