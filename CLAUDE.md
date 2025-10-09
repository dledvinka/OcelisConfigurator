# Ocelis Configurator - Project Documentation

## Project Overview

**Ocelis Configurator** is a Blazor Server web application built with .NET 9 that provides an interactive configurator for steel construction (ocelové konstrukce). The application allows users to input dimensions and roof types, calculates price estimates, and collects contact information for sales follow-up.

### Key Information
- **Framework:** .NET 9.0 (Blazor Server with Interactive Server Components)
- **UI Library:** MudBlazor 8.x
- **Language:** C# with Czech localization
- **Deployment:** Azure App Service (automated via GitHub Actions)
- **Repository:** https://github.com/dledvinka/OcelisConfigurator

---

## Solution Structure

```
OcelisConfigurator/
├── .github/
│   └── workflows/
│       └── main_ocelisconfigurator.yml    # Azure deployment pipeline
├── src/
│   ├── Ocelis.Configurator.sln            # Solution file
│   └── Ocelis.Configuration.BlazorApp/    # Main Blazor application
│       ├── Components/
│       │   ├── Pages/                     # Page components
│       │   │   ├── ConfiguratorFormPage.razor  # Main configurator form
│       │   │   ├── Home.razor             # Landing page (redirects)
│       │   │   ├── Counter.razor          # Sample page
│       │   │   ├── Weather.razor          # Sample page
│       │   │   └── Error.razor            # Error page
│       │   ├── Layout/                    # Layout components
│       │   │   ├── MainLayout.razor       # App layout with MudBlazor theme
│       │   │   └── NavMenu.razor          # Navigation menu
│       │   ├── Shared/                    # Shared components
│       │   │   └── DropDownItem.cs        # Dropdown model
│       │   ├── App.razor                  # Root app component
│       │   ├── Routes.razor               # Routing configuration
│       │   └── _Imports.razor             # Global using statements
│       ├── Domain/                        # Domain models
│       │   └── Length.cs                  # Length value object
│       ├── wwwroot/                       # Static assets
│       │   ├── images/                    # Image resources
│       │   │   ├── ocelis.svg             # Company logo
│       │   │   ├── RoofType_Flat.png      # Flat roof visualization
│       │   │   ├── RoofType_Pointy.png    # Pointy roof visualization
│       │   │   └── image-455-2-768x763.png # Alternate flat roof image
│       │   └── favicon.ico                # Site icon
│       ├── Properties/
│       │   └── launchSettings.json        # Development settings
│       ├── Program.cs                     # Application entry point
│       ├── appsettings.json               # Configuration
│       └── Ocelis.Configuration.BlazorApp.csproj  # Project file
└── README.md                              # Project readme
```

---

## Technical Architecture

### Technology Stack

1. **Backend Framework**
   - .NET 9.0
   - ASP.NET Core Blazor Server
   - Interactive Server Components (real-time server-side rendering)

2. **Frontend**
   - Blazor Components (Razor syntax)
   - MudBlazor 8.x for Material Design UI
   - Custom theming with light/dark mode support

3. **Key Dependencies**
   - `MudBlazor` (v8.*) - Material Design component library
   - `Microsoft.AspNetCore.Components` - Blazor framework
   - Built-in data annotations for validation

### Application Configuration

**Program.cs** ([Program.cs:1-34](src/Ocelis.Configuration.BlazorApp/Program.cs#L1-L34))
- Configures MudBlazor services
- Sets up Razor Components with Interactive Server mode
- Configures HTTPS redirection and antiforgery
- Maps static assets and Razor components

---

## Core Features

### 1. Steel Construction Configurator

**Main Form:** [ConfiguratorFormPage.razor](src/Ocelis.Configuration.BlazorApp/Components/Pages/ConfiguratorFormPage.razor)

The configurator allows users to:
- Input three dimensions (A, B, C) in millimeters
- Select roof type (Flat/Pointy - Rovná/Do špice)
- View real-time price estimates
- Submit contact information (phone, email)

#### Price Calculation Logic
```csharp
_priceEstimate = _model.DimensionA * _model.DimensionB * _model.DimensionC * _model.RoofType.Id / 1000;
```
Located in [ConfiguratorFormPage.razor:92](src/Ocelis.Configuration.BlazorApp/Components/Pages/ConfiguratorFormPage.razor#L92)

#### Visual Feedback
- Dynamic roof type images based on selection
- Real-time price updates on input changes
- Success notification on form submission using MudBlazor Snackbar

### 2. Form Data Model

**ConfiguratorFormInputData** ([ConfiguratorFormPage.razor:124-151](src/Ocelis.Configuration.BlazorApp/Components/Pages/ConfiguratorFormPage.razor#L124-L151))

Properties:
- `DimensionA`, `DimensionB`, `DimensionC` (decimal) - Required dimensions
- `RoofType` (DropDownItem) - Selected roof type
- `PhoneNumber` (string) - Required with custom error message
- `Email` (string) - Required with custom error message

Default values:
- All dimensions: 1000mm
- Roof type: Flat (Rovná)
- Phone: "+420123456789"
- Email: "muj.email@seznam.cz"

### 3. Roof Type Enumeration

**RoofType enum** ([ConfiguratorFormPage.razor:117-122](src/Ocelis.Configuration.BlazorApp/Components/Pages/ConfiguratorFormPage.razor#L117-L122))
- Unknown = 0
- Flat = 1 (Rovná)
- Pointy = 2 (Do špice)

---

## UI Components & Theming

### Main Layout

**MainLayout.razor** ([MainLayout.razor](src/Ocelis.Configuration.BlazorApp/Components/Layout/MainLayout.razor))

Features:
- MudBlazor theme provider with custom palettes
- App bar with Ocelis logo and title
- Support for light/dark mode (currently configured as light mode)
- Custom Figtree font family

#### Custom Theme Palette

**Light Mode Colors:**
- Primary: `#ff3938` (red)
- Black: `#110e2d`
- AppBar: White with 80% opacity
- Custom gray shades

**Dark Mode Colors:**
- Primary: `#ff3938` (red)
- Background: `#1a1a27`
- Surface: `#1e1e2d`
- Custom text and action colors

### Typography
- Font Family: Figtree, Helvetica, Arial, sans-serif
- Font Weight: 800 (bold by default)
- Loaded from Google Fonts

---

## Domain Models

### Length Value Object

**Location:** [Domain/Length.cs](src/Ocelis.Configuration.BlazorApp/Domain/Length.cs)

Simple value object for representing length measurements:
```csharp
public class Length
{
    public Length(decimal millimeters)
    {
        Millimeters = millimeters;
    }

    public decimal Millimeters { get; }
}
```

**Note:** Currently not used in the configurator form, which uses decimal directly.

### DropDownItem Record

**Location:** [Components/Shared/DropDownItem.cs](src/Ocelis.Configuration.BlazorApp/Components/Shared/DropDownItem.cs)

```csharp
public record DropDownItem(int Id, string DisplayText)
{
    public override string ToString() => DisplayText;
}
```

Used for roof type dropdown selections. Record type ensures immutability.

---

## User Flow

1. **Landing Page** ([Home.razor:67](src/Ocelis.Configuration.BlazorApp/Components/Pages/Home.razor#L67))
   - Automatically redirects to `/configurator-form`

2. **Configurator Form** (`/configurator-form`)
   - User enters dimensions (A, B, C)
   - User selects roof type
   - Price estimate updates in real-time
   - User enters contact information
   - Form validates inputs
   - On submit: Success message displayed

3. **Validation**
   - All dimension fields are required
   - Phone number required (Czech localization: "Prosím vyplňte telefonní číslo")
   - Email required (Czech localization: "Prosím vyplňte email")
   - Uses DataAnnotations validation

---

## Localization & Culture

The application is localized for Czech (Czech Republic):
- UI text in Czech language
- Currency formatting: Czech Koruna (cs-CZ culture)
- Price display: `_priceEstimate.ToString("C0", _cultureInfo)`
- Custom validation messages in Czech

---

## Deployment & CI/CD

### GitHub Actions Workflow

**File:** [.github/workflows/main_ocelisconfigurator.yml](.github/workflows/main_ocelisconfigurator.yml)

**Trigger:** Push to `main` branch or manual workflow dispatch

**Build Job:**
- Runs on: Ubuntu latest
- Working directory: `./src`
- .NET version: 9.x
- Steps:
  1. Checkout code
  2. Setup .NET Core 9.x
  3. Build in Release configuration
  4. Publish application
  5. Upload build artifact

**Deploy Job:**
- Runs on: Ubuntu latest
- Depends on: Build job
- Environment: Production
- Azure authentication using OIDC (OpenID Connect)
- Deploys to Azure Web App: "OcelisConfigurator"
- Slot: Production

### Azure Configuration

Uses Azure secrets for authentication:
- `AZUREAPPSERVICE_CLIENTID`
- `AZUREAPPSERVICE_TENANTID`
- `AZUREAPPSERVICE_SUBSCRIPTIONID`

---

## Development History

Recent commits (from git log):
1. `878e67f` - Solve issue with DropDownItem
2. `aa5f198` - Merge branch 'main'
3. `e17f7de` - Tuning
4. `f2f6cca` - Update main_ocelisconfigurator.yml
5. `035bf12` - Add or update Azure App Service workflow
6. `1be05a1` - Edit form
7. `4943078` - Project created
8. `00a1194` - Initial commit

---

## Key Files Reference

### Entry Points
- [Program.cs](src/Ocelis.Configuration.BlazorApp/Program.cs) - Application startup
- [App.razor](src/Ocelis.Configuration.BlazorApp/Components/App.razor) - Root component
- [Routes.razor](src/Ocelis.Configuration.BlazorApp/Components/Routes.razor) - Routing

### Main Pages
- [Home.razor](src/Ocelis.Configuration.BlazorApp/Components/Pages/Home.razor) - Landing (redirects)
- [ConfiguratorFormPage.razor](src/Ocelis.Configuration.BlazorApp/Components/Pages/ConfiguratorFormPage.razor) - Main form

### Layout
- [MainLayout.razor](src/Ocelis.Configuration.BlazorApp/Components/Layout/MainLayout.razor) - App layout
- [NavMenu.razor](src/Ocelis.Configuration.BlazorApp/Components/Layout/NavMenu.razor) - Navigation

### Models
- [Length.cs](src/Ocelis.Configuration.BlazorApp/Domain/Length.cs) - Length value object
- [DropDownItem.cs](src/Ocelis.Configuration.BlazorApp/Components/Shared/DropDownItem.cs) - Dropdown model

### Configuration
- [appsettings.json](src/Ocelis.Configuration.BlazorApp/appsettings.json) - App settings
- [Ocelis.Configuration.BlazorApp.csproj](src/Ocelis.Configuration.BlazorApp/Ocelis.Configuration.BlazorApp.csproj) - Project file

---

## Future Improvements & Considerations

### Potential Enhancements

1. **Domain Models**
   - The `Length` class is defined but not currently used
   - Could replace raw decimal values for better type safety

2. **Form Submission**
   - Currently only shows success message
   - Could integrate with backend API/database to store inquiries
   - Could send email notifications to sales team

3. **Price Calculation**
   - Simple formula: `A * B * C * RoofType / 1000`
   - Could be enhanced with more sophisticated pricing logic
   - Could factor in material costs, labor, complexity

4. **Validation**
   - Could add range validation for dimensions
   - Could validate phone number format
   - Could validate email format

5. **Internationalization**
   - Currently Czech only
   - Could add multi-language support

6. **Features**
   - Add more roof types
   - Add material selection
   - Add 3D visualization
   - Add PDF quote generation
   - Add save/share configuration

---

## MudBlazor Components Used

- `MudGrid` / `MudItem` - Responsive grid layout
- `MudCard` / `MudCardContent` / `MudCardActions` - Card containers
- `MudNumericField` - Numeric input with validation
- `MudSelect` / `MudSelectItem` - Dropdown selection
- `MudTextField` - Text input
- `MudButton` - Buttons
- `MudText` - Typography
- `MudImage` - Image display
- `MudAppBar` - Application bar
- `MudSnackbarProvider` / `ISnackbar` - Toast notifications
- `MudThemeProvider` - Theming
- `MudPopoverProvider` / `MudDialogProvider` - Modals and popovers
- `MudLayout` / `MudMainContent` - Layout structure

---

## Running the Application

### Development
```bash
cd src/Ocelis.Configuration.BlazorApp
dotnet run
```

### Build
```bash
cd src
dotnet build --configuration Release
```

### Publish
```bash
cd src
dotnet publish -c Release -o ./publish
```

---

## Summary

The Ocelis Configurator is a focused, single-purpose Blazor Server application that provides an interactive steel construction configurator with real-time price estimation. It demonstrates modern .NET 9 Blazor patterns, MudBlazor component integration, and automated Azure deployment. The application is production-ready with form validation, responsive design, and a polished user experience.
