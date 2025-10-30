using Ocelis.Configuration.Domain.Entities;

namespace Ocelis.Configuration.BlazorApp.Domain;

public class EmailMessageModel
{
    public Zakazka Zakazka { get; set; }
    public ZakazkaCena ZakazkaCena { get; set; }
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
}