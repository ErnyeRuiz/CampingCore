using System.Net;
using System.Text;
using Microsoft.Extensions.Options;

namespace CampingCore.Application.Email;

public static class TransactionalEmailHtml
{
    public static string BuildLogoAbsoluteUrl(EmailBrandingOptions options)
    {
        var baseUrl = options.PublicBaseUrl.TrimEnd('/');
        if (string.IsNullOrEmpty(baseUrl))
            return string.Empty;

        var relative = options.LogoRelativePath.TrimStart('/');
        var segments = relative.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (segments.Length == 0)
            return baseUrl;

        var encoded = string.Join("/", segments.Select(Uri.EscapeDataString));
        return $"{baseUrl}/{encoded}";
    }

    public static string VerificationEmail(IOptions<EmailBrandingOptions> branding, string recipientName, string verificationCode)
        => VerificationEmail(branding.Value, recipientName, verificationCode);

    public static string VerificationEmail(EmailBrandingOptions options, string recipientName, string verificationCode)
    {
        var safeName = WebUtility.HtmlEncode(recipientName);
        var safeCode = WebUtility.HtmlEncode(verificationCode);
        var accent = WebUtility.HtmlEncode(options.AccentColor.Trim());
        var logoUrl = BuildLogoAbsoluteUrl(options);

        var sb = new StringBuilder();
        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html lang=\"es\">");
        sb.AppendLine("<head>");
        sb.AppendLine("  <meta charset=\"utf-8\" />");
        sb.AppendLine("  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\" />");
        sb.AppendLine("  <title>Verificá tu cuenta</title>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body style=\"margin:0;padding:0;background-color:#faf7f4;\">");
        sb.AppendLine("  <table role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\" width=\"100%\" style=\"background-color:#faf7f4;\">");
        sb.AppendLine("    <tr>");
        sb.AppendLine("      <td align=\"center\" style=\"padding:32px 16px;\">");
        sb.AppendLine("        <table role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\" width=\"100%\" style=\"max-width:600px;background-color:#ffffff;border-radius:8px;overflow:hidden;\">");

        if (!string.IsNullOrEmpty(logoUrl))
        {
            sb.AppendLine("          <tr>");
            sb.AppendLine("            <td align=\"center\" style=\"padding:24px 24px 8px 24px;\">");
            sb.Append("              <img src=\"").Append(WebUtility.HtmlEncode(logoUrl)).Append("\" alt=\"CampingCore\" width=\"180\" style=\"display:block;max-width:180px;height:auto;border:0;\" />");
            sb.AppendLine();
            sb.AppendLine("            </td>");
            sb.AppendLine("          </tr>");
        }

        sb.AppendLine("          <tr>");
        sb.AppendLine("            <td align=\"center\" style=\"padding:16px 32px 8px 32px;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,'Helvetica Neue',Arial,sans-serif;font-size:26px;line-height:1.25;color:#1f2937;font-weight:700;\">");
        sb.AppendLine("              Verificá tu cuenta");
        sb.AppendLine("            </td>");
        sb.AppendLine("          </tr>");
        sb.AppendLine("          <tr>");
        sb.AppendLine("            <td align=\"center\" style=\"padding:8px 32px 16px 32px;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,'Helvetica Neue',Arial,sans-serif;font-size:16px;line-height:1.6;color:#374151;\">");
        sb.Append("              Hola ").Append(safeName).Append(",<br /><br />");
        sb.AppendLine();
        sb.AppendLine("              Gracias por registrarte en CampingCore. Usá este código para confirmar tu correo:");
        sb.AppendLine("            </td>");
        sb.AppendLine("          </tr>");
        sb.AppendLine("          <tr>");
        sb.AppendLine("            <td align=\"center\" style=\"padding:8px 32px 24px 32px;\">");
        sb.AppendLine("              <table role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"margin:0 auto;\">");
        sb.AppendLine("                <tr>");
        sb.AppendLine("                  <td align=\"center\" style=\"background-color:#f3f4f6;border-radius:8px;padding:16px 28px;font-family:Consolas,'Courier New',monospace;font-size:28px;font-weight:700;letter-spacing:0.15em;color:#111827;\">");
        sb.Append("                    ").Append(safeCode).AppendLine();
        sb.AppendLine("                  </td>");
        sb.AppendLine("                </tr>");
        sb.AppendLine("              </table>");
        sb.AppendLine("            </td>");
        sb.AppendLine("          </tr>");
        sb.AppendLine("          <tr>");
        sb.AppendLine("            <td align=\"center\" style=\"padding:0 32px 28px 32px;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,'Helvetica Neue',Arial,sans-serif;font-size:15px;line-height:1.6;color:#4b5563;\">");
        sb.AppendLine("              El código vence en <strong>15 minutos</strong>. Si no creaste una cuenta, podés ignorar este mensaje.");
        sb.AppendLine("            </td>");
        sb.AppendLine("          </tr>");
        sb.AppendLine("          <tr>");
        sb.Append("            <td style=\"height:1px;background-color:").Append(accent).Append(";line-height:1px;font-size:1px;\">&nbsp;</td>");
        sb.AppendLine();
        sb.AppendLine("          </tr>");

        if (!string.IsNullOrWhiteSpace(options.LegalNotice))
        {
            sb.AppendLine("          <tr>");
            sb.AppendLine("            <td align=\"center\" style=\"padding:0 24px 16px 24px;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,'Helvetica Neue',Arial,sans-serif;font-size:12px;line-height:1.5;color:#6b7280;\">");
            sb.Append("              ").Append(WebUtility.HtmlEncode(options.LegalNotice)).AppendLine();
            sb.AppendLine("            </td>");
            sb.AppendLine("          </tr>");
        }

        AppendSupportFooter(sb, options, accent);

        sb.AppendLine("        </table>");
        sb.AppendLine("      </td>");
        sb.AppendLine("    </tr>");
        sb.AppendLine("  </table>");
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");

        return sb.ToString();
    }

    private static void AppendSupportFooter(StringBuilder sb, EmailBrandingOptions options, string accentEncoded)
    {
        var hasEmail = !string.IsNullOrWhiteSpace(options.SupportEmail);
        var hasFaq = !string.IsNullOrWhiteSpace(options.SupportFaqUrl);

        if (!hasEmail && !hasFaq)
        {
            sb.AppendLine("          <tr>");
            sb.AppendLine("            <td align=\"center\" style=\"padding:20px 24px 28px 24px;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,'Helvetica Neue',Arial,sans-serif;font-size:14px;line-height:1.6;color:#374151;\">");
            sb.AppendLine("              ¡Gracias por tu tiempo!");
            sb.AppendLine("            </td>");
            sb.AppendLine("          </tr>");
            return;
        }

        var faqPart = hasFaq
            ? $"<a href=\"{WebUtility.HtmlEncode(options.SupportFaqUrl!.Trim())}\" style=\"color:{accentEncoded};text-decoration:none;font-weight:600;\">Preguntas frecuentes</a>"
            : "";

        var mailPart = hasEmail
            ? $"<a href=\"mailto:{WebUtility.HtmlEncode(options.SupportEmail!.Trim())}\" style=\"color:{accentEncoded};text-decoration:none;font-weight:600;\">{WebUtility.HtmlEncode(options.SupportEmail!.Trim())}</a>"
            : "";

        string sentence;
        if (hasFaq && hasEmail)
            sentence = $"Si tenés dudas, visitá nuestras {faqPart} o escribinos a {mailPart}.";
        else if (hasFaq)
            sentence = $"Si tenés dudas, visitá nuestras {faqPart}.";
        else
            sentence = $"Si tenés dudas, escribinos a {mailPart}.";

        sb.AppendLine("          <tr>");
        sb.AppendLine("            <td align=\"center\" style=\"padding:20px 24px 12px 24px;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,'Helvetica Neue',Arial,sans-serif;font-size:14px;line-height:1.6;color:#374151;font-weight:600;\">");
        sb.AppendLine("              ¡Gracias por tu tiempo!");
        sb.AppendLine("            </td>");
        sb.AppendLine("          </tr>");
        sb.AppendLine("          <tr>");
        sb.AppendLine("            <td align=\"center\" style=\"padding:0 24px 28px 24px;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,'Helvetica Neue',Arial,sans-serif;font-size:13px;line-height:1.65;color:#6b7280;\">");
        sb.Append("              ").Append(sentence).AppendLine();
        sb.AppendLine("            </td>");
        sb.AppendLine("          </tr>");
    }
}
