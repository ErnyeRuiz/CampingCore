using CampingCore.Application.Abstractions.Messaging;
using CampingCore.Domain.Events;
using CampingCore.Domain.Repositories;

namespace CampingCore.Application.Events;

public sealed class CampSiteRatingRecalculationRequestedDomainEventHandler
    : IDomainEventHandler<CampSiteRatingRecalculationRequestedDomainEvent>
{
    private readonly ICampSiteRepository _campSiteRepository;

    public CampSiteRatingRecalculationRequestedDomainEventHandler(ICampSiteRepository campSiteRepository)
        => _campSiteRepository = campSiteRepository;

    public Task Handle(CampSiteRatingRecalculationRequestedDomainEvent notification, CancellationToken cancellationToken)
        => _campSiteRepository.RecalculateCampSiteRatingAsync(notification.CampSiteId, cancellationToken);
}
