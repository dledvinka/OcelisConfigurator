namespace Ocelis.Configuration.BlazorApp.Services;

public interface IEmailService
{
    Task<bool> SendConfiguratorInquiryAsync(
        decimal dimensionA,
        decimal dimensionB,
        decimal dimensionC,
        string roofType,
        string phoneNumber,
        string email,
        decimal priceEstimate);
}
