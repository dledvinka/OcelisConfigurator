using System.Globalization;
using System.Net;
using System.Net.Mail;

namespace Ocelis.Configuration.BlazorApp.Services;

public class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(
        IConfiguration configuration,
        ILogger<SmtpEmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> SendConfiguratorInquiryAsync(
        decimal dimensionA,
        decimal dimensionB,
        decimal dimensionC,
        string roofType,
        string phoneNumber,
        string email,
        decimal priceEstimate)
    {
        try
        {
            // Get SMTP configuration
            var smtpHost = _configuration["Email:SmtpHost"]
                ?? throw new InvalidOperationException("SMTP Host is not configured");
            var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
            var smtpUsername = _configuration["Email:SmtpUsername"];
            var smtpPassword = _configuration["Email:SmtpPassword"];
            var fromEmail = _configuration["Email:FromEmail"]
                ?? throw new InvalidOperationException("FromEmail is not configured");
            var fromName = _configuration["Email:FromName"] ?? "Ocelis Konfigurátor";
            var toEmail = _configuration["Email:ToEmail"]
                ?? throw new InvalidOperationException("ToEmail (recipient) is not configured");
            var enableSsl = bool.Parse(_configuration["Email:EnableSsl"] ?? "true");

            // Create mail message
            using var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = $"Nová poptávka z konfigurátoru - {email}",
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);
            mailMessage.ReplyToList.Add(new MailAddress(email)); // Allow easy reply to customer

            // Build email body
            mailMessage.Body = BuildHtmlEmailBody(
                dimensionA, dimensionB, dimensionC, roofType,
                phoneNumber, email, priceEstimate);

            // Add plain text alternative
            var plainTextView = AlternateView.CreateAlternateViewFromString(
                BuildPlainTextEmailBody(dimensionA, dimensionB, dimensionC, roofType,
                    phoneNumber, email, priceEstimate),
                null,
                "text/plain");
            mailMessage.AlternateViews.Add(plainTextView);

            // Configure SMTP client
            using var smtpClient = new SmtpClient(smtpHost, smtpPort)
            {
                EnableSsl = enableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };

            // Only set credentials if username is provided
            if (!string.IsNullOrEmpty(smtpUsername))
            {
                smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
            }

            // Send email
            await smtpClient.SendMailAsync(mailMessage);

            _logger.LogInformation(
                "Email sent successfully to {ToEmail} for inquiry from {CustomerEmail}",
                toEmail, email);

            return true;
        }
        catch (SmtpException ex)
        {
            _logger.LogError(ex,
                "SMTP error sending email for inquiry from {Email}. SMTP Status: {Status}",
                email, ex.StatusCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email for inquiry from {Email}", email);
            return false;
        }
    }

    private string BuildHtmlEmailBody(
        decimal dimensionA, decimal dimensionB, decimal dimensionC,
        string roofType, string phoneNumber, string email, decimal priceEstimate)
    {
        var cultureInfo = new CultureInfo("cs-CZ");
        var volume = (dimensionA * dimensionB * dimensionC) / 1_000_000_000; // Convert to m³

        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #ff3938; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }}
        .content {{ background-color: #f9f9f9; padding: 20px; border: 1px solid #ddd; border-radius: 0 0 5px 5px; }}
        .info-row {{ padding: 10px 0; border-bottom: 1px solid #ddd; }}
        .info-row:last-child {{ border-bottom: none; }}
        .label {{ font-weight: bold; color: #110e2d; display: inline-block; width: 150px; }}
        .value {{ color: #424242; }}
        .dimensions {{ background-color: #fff; padding: 15px; margin: 15px 0; border-left: 4px solid #ff3938; border-radius: 3px; }}
        .price {{ font-size: 28px; color: #ff3938; font-weight: bold; margin: 20px 0; text-align: center; padding: 15px; background: #fff; border-radius: 5px; }}
        .footer {{ margin-top: 20px; padding-top: 20px; border-top: 2px solid #ddd; font-size: 12px; color: #666; text-align: center; }}
        h2 {{ color: #110e2d; border-bottom: 2px solid #ff3938; padding-bottom: 10px; }}
        a {{ color: #ff3938; text-decoration: none; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1 style='margin: 0; font-size: 24px;'>📋 Nová poptávka z konfigurátoru</h1>
        </div>

        <div class='content'>
            <h2>👤 Kontaktní údaje</h2>
            <div class='info-row'>
                <span class='label'>Email:</span>
                <span class='value'><a href='mailto:{email}'>{email}</a></span>
            </div>
            <div class='info-row'>
                <span class='label'>Telefon:</span>
                <span class='value'><a href='tel:{phoneNumber}'>{phoneNumber}</a></span>
            </div>

            <h2>📐 Požadované parametry</h2>
            <div class='dimensions'>
                <div class='info-row'>
                    <span class='label'>Rozměr A:</span>
                    <span class='value'><strong>{dimensionA:N0} mm</strong></span>
                </div>
                <div class='info-row'>
                    <span class='label'>Rozměr B:</span>
                    <span class='value'><strong>{dimensionB:N0} mm</strong></span>
                </div>
                <div class='info-row'>
                    <span class='label'>Rozměr C:</span>
                    <span class='value'><strong>{dimensionC:N0} mm</strong></span>
                </div>
                <div class='info-row'>
                    <span class='label'>Celkový objem:</span>
                    <span class='value'><strong>{volume:N2} m³</strong></span>
                </div>
                <div class='info-row'>
                    <span class='label'>Typ střechy:</span>
                    <span class='value'><strong>{roofType}</strong></span>
                </div>
            </div>

            <h2>💰 Odhadovaná cena</h2>
            <div class='price'>
                {priceEstimate.ToString("C0", cultureInfo)}
            </div>

            <div class='footer'>
                <p>✉️ Tento email byl automaticky vygenerován z konfigurátoru ocelových konstrukcí.</p>
                <p>💡 Pro odpověď zákazníkovi stačí kliknout na tlačítko 'Odpovědět' - email bude automaticky odeslán na <strong>{email}</strong></p>
                <p style='color: #999; margin-top: 15px;'>Datum odeslání: {DateTime.Now:dd.MM.yyyy HH:mm}</p>
            </div>
        </div>
    </div>
</body>
</html>";
    }

    private string BuildPlainTextEmailBody(
        decimal dimensionA, decimal dimensionB, decimal dimensionC,
        string roofType, string phoneNumber, string email, decimal priceEstimate)
    {
        var cultureInfo = new CultureInfo("cs-CZ");
        var volume = (dimensionA * dimensionB * dimensionC) / 1_000_000_000;

        return $@"
═══════════════════════════════════════
  NOVÁ POPTÁVKA Z KONFIGURÁTORU
═══════════════════════════════════════

KONTAKTNÍ ÚDAJE:
────────────────
Email:    {email}
Telefon:  {phoneNumber}

POŽADOVANÉ PARAMETRY:
────────────────────
Rozměr A:      {dimensionA:N0} mm
Rozměr B:      {dimensionB:N0} mm
Rozměr C:      {dimensionC:N0} mm
Celkový objem: {volume:N2} m³
Typ střechy:   {roofType}

ODHADOVANÁ CENA:
────────────────
{priceEstimate.ToString("C0", cultureInfo)}

═══════════════════════════════════════
Tento email byl automaticky vygenerován
z konfigurátoru ocelových konstrukcí.

Pro odpověď zákazníkovi stačí odpovědět
na tento email.

Datum: {DateTime.Now:dd.MM.yyyy HH:mm}
═══════════════════════════════════════
";
    }
}
