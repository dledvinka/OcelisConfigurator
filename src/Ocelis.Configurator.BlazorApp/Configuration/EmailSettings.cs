namespace Ocelis.Configuration.BlazorApp.Configuration;

public class EmailSettings
{
    public string SmtpHost { get; set; } = string.Empty;
    public int SmtpPort { get; set; } = 587;
    public string SmtpUsername { get; set; } = string.Empty;
    public string SmtpPassword { get; set; } = string.Empty;
    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = "OCELIS Konfigurátor";
    public string ToEmail { get; set; } = string.Empty;
    public bool EnableSsl { get; set; } = true;
}
