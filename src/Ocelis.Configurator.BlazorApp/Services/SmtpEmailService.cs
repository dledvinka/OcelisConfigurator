namespace Ocelis.Configuration.BlazorApp.Services;

using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Text;
using Ocelis.Configuration.BlazorApp.Configuration;
using Ocelis.Configuration.BlazorApp.Domain;

public class SmtpEmailService : IEmailService
{
    private readonly CompanySettings _companySettings;
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(
        EmailSettings emailSettings,
        CompanySettings companySettings,
        ILogger<SmtpEmailService> logger)
    {
        _emailSettings = emailSettings;
        _companySettings = companySettings;
        _logger = logger;
    }

    public bool IsValidEmail(string emailName) => new EmailAddressAttribute().IsValid(emailName);

    public async Task<bool> SendEmailAsync(string toEmailName, EmailMessageModel messageModel)
    {
        try
        {
            // Validate email settings
            if (string.IsNullOrEmpty(_emailSettings.SmtpHost))
                throw new InvalidOperationException("SMTP Host is not configured");
            if (string.IsNullOrEmpty(_emailSettings.FromEmail))
                throw new InvalidOperationException("FromEmail is not configured");
            if (string.IsNullOrEmpty(_emailSettings.ToEmail))
                throw new InvalidOperationException("ToEmail (manager) is not configured");

            // Configure SMTP client
            using var smtpClient = new SmtpClient(_emailSettings.SmtpHost, _emailSettings.SmtpPort);
            smtpClient.EnableSsl = _emailSettings.EnableSsl;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.UseDefaultCredentials = false;

            // Only set credentials if username is provided
            if (!string.IsNullOrEmpty(_emailSettings.SmtpUsername))
                smtpClient.Credentials = new NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);

            // 1. Send detailed email to MANAGER
            var managerMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName),
                Subject = $"Nová poptávka z konfigurátoru - {messageModel.CustomerEmail}",
                SubjectEncoding = Encoding.UTF8,
                BodyEncoding = Encoding.UTF8,
                HeadersEncoding = Encoding.UTF8,
                IsBodyHtml = true,
                Body = BuildManagerEmailBody(messageModel, messageModel.CustomerEmail, messageModel.CustomerPhone),
                Priority = MailPriority.High
            };

            managerMessage.To.Add(new MailAddress(_emailSettings.ToEmail));
            managerMessage.ReplyToList.Add(new MailAddress(messageModel.CustomerEmail)); // Easy reply to customer

            await smtpClient.SendMailAsync(managerMessage);

            _logger.LogInformation("Manager email sent successfully to {ManagerEmail} for inquiry from {CustomerEmail}",
                                   _emailSettings.ToEmail, messageModel.CustomerEmail);

            // 2. Send confirmation email to CUSTOMER
            var customerMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName),
                Subject = "Potvrzení poptávky - OCELIS Konfigurátor",
                SubjectEncoding = Encoding.UTF8,
                BodyEncoding = Encoding.UTF8,
                HeadersEncoding = Encoding.UTF8,
                IsBodyHtml = true,
                Body = BuildCustomerEmailBody(messageModel),
                Priority = MailPriority.Normal
            };

            customerMessage.To.Add(new MailAddress(messageModel.CustomerEmail));

            await smtpClient.SendMailAsync(customerMessage);

            _logger.LogInformation("Customer confirmation email sent successfully to {CustomerEmail}",
                                   messageModel.CustomerEmail);

            return true;
        }
        catch (SmtpException ex)
        {
            _logger.LogError(ex,
                             "SMTP error sending emails. SMTP Status: {Status}",
                             ex.StatusCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending emails");
            return false;
        }
    }

    private string LoadEmailTemplate(string templateName)
    {
        try
        {
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), templateName);

            using var fs = File.Open(templatePath, FileMode.Open, FileAccess.Read);
            using var sr = new StreamReader(fs);
            return sr.ReadToEnd();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading email template from {TemplatePath}", templateName);
            throw;
        }
    }

    private string BuildManagerEmailBody(EmailMessageModel messageModel, string customerEmail, string customerPhone)
    {
        var cultureInfo = new CultureInfo("cs-CZ");
        var zakazka = messageModel.Zakazka;
        var cena = messageModel.ZakazkaCena;

        var htmlBody = LoadEmailTemplate("Email_Template_Manager.html");

        // Customer info
        htmlBody = htmlBody.Replace("###CUSTOMEREMAIL###", customerEmail);
        htmlBody = htmlBody.Replace("###CUSTOMERPHONE###", customerPhone);

        // Zakazka basic info
        htmlBody = htmlBody.Replace("###STAVBATYP###", zakazka.StavbaTyp.ToString());
        htmlBody = htmlBody.Replace("###VAZNIKTYP###", zakazka.VaznikTyp.ToString());
        htmlBody = htmlBody.Replace("###POCETVELKYCHOTVORU###", zakazka.PocetVelkychOtvoru.ToString());
        htmlBody = htmlBody.Replace("###CPROFILTYP###", zakazka.CProfilTyp?.Kod ?? "N/A");

        // Dimensions
        htmlBody = htmlBody.Replace("###DELKA###", zakazka.Delka.Milimetry.ToString("N0", cultureInfo));
        htmlBody = htmlBody.Replace("###SIRKA###", zakazka.Sirka.Milimetry.ToString("N0", cultureInfo));
        htmlBody = htmlBody.Replace("###SVETLAVYSKASTEM###", zakazka.SvetlaVyskaSten.Milimetry.ToString("N0", cultureInfo));

        // Price breakdown
        htmlBody = htmlBody.Replace("###CENAOCELOVAKONSTRUKCE###", (cena.CenaOcelovaKonstrukceOcelisCzk ?? 0).ToString("N0", cultureInfo));
        htmlBody = htmlBody.Replace("###CENASILNOSTENNAKONSTRUKCE###", (cena.CenaSilnostennaKonstrukceCzk ?? 0).ToString("N0", cultureInfo));
        htmlBody = htmlBody.Replace("###CENAOPLASTENI###", (cena.CenaOplasteniCzk ?? 0).ToString("N0", cultureInfo));
        htmlBody = htmlBody.Replace("###CENAMONTAZ###", (cena.CenaMontazNaStavbeCzk ?? 0).ToString("N0", cultureInfo));
        htmlBody = htmlBody.Replace("###CENAMANIPULACE###", (cena.CenaManipulacniTechnikaCzk ?? 0).ToString("N0", cultureInfo));
        htmlBody = htmlBody.Replace("###CENASPOJOVACI###", (cena.CenaSpojovaciMaterialCzk ?? 0).ToString("N0", cultureInfo));
        htmlBody = htmlBody.Replace("###CENACELKEM###", (cena.CenaCelkemCzk ?? 0).ToString("C0", cultureInfo));

        // Popis (description/error message)
        var popisHtml = "";
        if (!string.IsNullOrEmpty(cena.Popis))
            popisHtml = $"<div class='error-message'><strong>Poznámka:</strong> {cena.Popis}</div>";
        htmlBody = htmlBody.Replace("###POPIS###", popisHtml);

        htmlBody = htmlBody.Replace("###CURRENTDATE###", DateTime.Now.ToString("dd.MM.yyyy HH:mm"));

        return htmlBody;
    }

    private string BuildCustomerEmailBody(EmailMessageModel messageModel)
    {
        var cultureInfo = new CultureInfo("cs-CZ");
        var zakazka = messageModel.Zakazka;
        var cena = messageModel.ZakazkaCena;

        var htmlBody = LoadEmailTemplate("Email_Template_Customer.html");

        // Zakazka limited info (only what customer should see)
        htmlBody = htmlBody.Replace("###STAVBATYP###", zakazka.StavbaTyp.ToString());
        htmlBody = htmlBody.Replace("###VAZNIKTYP###", zakazka.VaznikTyp.ToString());
        htmlBody = htmlBody.Replace("###POCETVELKYCHOTVORU###", zakazka.PocetVelkychOtvoru.ToString());

        // Dimensions
        htmlBody = htmlBody.Replace("###DELKA###", zakazka.Delka.Milimetry.ToString("N0", cultureInfo));
        htmlBody = htmlBody.Replace("###SIRKA###", zakazka.Sirka.Milimetry.ToString("N0", cultureInfo));
        htmlBody = htmlBody.Replace("###SVETLAVYSKASTEM###", zakazka.SvetlaVyskaSten.Milimetry.ToString("N0", cultureInfo));

        var objem = (zakazka.Delka.Milimetry * zakazka.Sirka.Milimetry * zakazka.SvetlaVyskaSten.Milimetry) / 1_000_000_000;
        htmlBody = htmlBody.Replace("###OBJEM###", objem.ToString("N2", cultureInfo));

        // Price
        htmlBody = htmlBody.Replace("###CENACELKEM###", (cena.CenaCelkemCzk ?? 0).ToString("C0", cultureInfo));

        // Popis (error message if any)
        var popisHtml = "";
        if (!string.IsNullOrEmpty(cena.Popis))
            popisHtml = $"<div class='error-message'><strong>⚠️ Upozornění:</strong> {cena.Popis}</div>";
        htmlBody = htmlBody.Replace("###POPIS###", popisHtml);

        // Company contact info
        htmlBody = htmlBody.Replace("###COMPANYNAME###", _companySettings.Name);
        htmlBody = htmlBody.Replace("###COMPANYEMAIL###", _companySettings.Email);
        htmlBody = htmlBody.Replace("###COMPANYPHONE###", _companySettings.Phone);
        htmlBody = htmlBody.Replace("###COMPANYADDRESS###", _companySettings.Address);

        htmlBody = htmlBody.Replace("###CURRENTDATE###", DateTime.Now.ToString("dd.MM.yyyy HH:mm"));

        return htmlBody;
    }
}