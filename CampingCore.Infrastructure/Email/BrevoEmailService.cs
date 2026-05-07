using CampingCore.Domain.Entities.System;
using CampingCore.Domain.Repositories;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Net.Http.Json;

namespace CampingCore.Infrastructure.Email
{
    public class BrevoEmailService : IEmailService
    {
        private readonly HttpClient _httpClient;
        private readonly BrevoSettings _settings;

        public BrevoEmailService(HttpClient httpClient, IOptions<BrevoSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
        }

        public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
        {
            var payload = new
            {
                sender = new
                {
                    email = _settings.SenderEmail,
                    name = _settings.SenderName
                },
                to = new[]
                {
                new
                {
                    email = message.ToEmail,
                    name = message.ToName
                }
            },
                subject = message.Subject,
                htmlContent = message.HtmlContent
            };

            var response = await _httpClient.PostAsJsonAsync("v3/smtp/email", payload, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                Debug.WriteLine($"[Brevo] Error body: {error}");
                throw new Exception($"Brevo error {response.StatusCode}: {error}");
            }
        }
    }
}
