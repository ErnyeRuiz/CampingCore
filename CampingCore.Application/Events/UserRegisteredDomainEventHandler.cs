using CampingCore.Application.Email;
using CampingCore.Domain.Entities.System;
using CampingCore.Domain.Events;
using CampingCore.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Options;

namespace CampingCore.Application.Events;

public sealed class UserRegisteredDomainEventHandler : INotificationHandler<UserRegisteredDomainEvent>
{
    private readonly IEmailService _emailService;
    private readonly IOptions<EmailBrandingOptions> _emailBranding;

    public UserRegisteredDomainEventHandler(
        IEmailService emailService,
        IOptions<EmailBrandingOptions> emailBranding)
    {
        _emailService = emailService;
        _emailBranding = emailBranding;
    }

    public Task Handle(UserRegisteredDomainEvent notification, CancellationToken cancellationToken)
    {
        var html = TransactionalEmailHtml.VerificationEmail(_emailBranding, notification.Name, notification.Code);

        var message = new EmailMessage(
            notification.Email,
            notification.Name,
            "Verificá tu cuenta",
            html);

        return _emailService.SendAsync(message, cancellationToken);
    }
}
