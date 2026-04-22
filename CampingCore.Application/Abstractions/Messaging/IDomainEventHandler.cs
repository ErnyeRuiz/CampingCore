using CampingCore.Domain.Primitives;
using MediatR;

namespace CampingCore.Application.Abstractions.Messaging;

public interface IDomainEventHandler<TDomainEvent> : INotificationHandler<TDomainEvent>
    where TDomainEvent : IDomainEvent, INotification
{
}
