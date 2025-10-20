using MudBlazor;
using MudBlazor.Services;
using Ocelis.Configuration.BlazorApp.Components;
using Ocelis.Configuration.BlazorApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services.AddMudServices();

// Add Email service
builder.Services.AddScoped<IEmailService, SmtpEmailService>();

// Add Pricing services
builder.Services.AddSingleton<PricingConfiguration>();
builder.Services.AddScoped<PricingCalculationService>();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Load pricing configuration at startup
var pricingConfig = app.Services.GetRequiredService<PricingConfiguration>();
await pricingConfig.LoadAsync();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
