using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampingCore.Infrastructure.Email
{
    public class BrevoSettings
    {
        public const string SectionName = "Brevo";

        public string ApiKey { get; init; } = string.Empty;
        public string SenderEmail { get; init; } = string.Empty;
        public string SenderName { get; init; } = string.Empty;
    }
}
