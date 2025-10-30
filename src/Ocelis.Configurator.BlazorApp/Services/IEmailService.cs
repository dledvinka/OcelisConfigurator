namespace Ocelis.Configuration.BlazorApp.Services;

using Ocelis.Configuration.BlazorApp.Domain;

public interface IEmailService
{
    Task<bool> SendEmailAsync(string toEmailName, EmailMessageModel messageModel);
    bool IsValidEmail(string emailName);
}