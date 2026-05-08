using CampingCore.Application.Email;
using CampingCore.Domain.Entities.System;
using CampingCore.Domain.Events;
using CampingCore.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Options;

namespace CampingCore.Application.Events;

public sealed class PasswordResetRequestedDomainEventHandler : INotificationHandler<PasswordResetRequestedDomainEvent>
{
    private readonly IEmailService _emailService;
    private readonly IOptions<EmailBrandingOptions> _emailBranding;
    private readonly IOptions<FrontendOptions> _frontend;

    public PasswordResetRequestedDomainEventHandler(
        IEmailService emailService,
        IOptions<EmailBrandingOptions> emailBranding,
        IOptions<FrontendOptions> frontend)
    {
        _emailService = emailService;
        _emailBranding = emailBranding;
        _frontend = frontend;
    }

    public Task Handle(PasswordResetRequestedDomainEvent notification, CancellationToken cancellationToken)
    {
        var baseUrl = _frontend.Value.BaseUrl.TrimEnd('/');
        var resetLink = $"{baseUrl}/reset-password?code={Uri.EscapeDataString(notification.PlainCode)}";
        var html = TransactionalEmailHtml.PasswordResetEmail(
            _emailBranding,
            notification.Name,
            notification.PlainCode,
            resetLink);

        var message = new EmailMessage(
            notification.Email,
            notification.Name,
            "Restablecé tu contraseña",
            html);

        return _emailService.SendAsync(message, cancellationToken);
    }
}
