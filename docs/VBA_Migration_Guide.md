# VBA to C# Migration Guide - Ocelis Pricing Calculator

## Overview

This document describes the successful migration of Excel VBA pricing calculation logic from `Tabulka CN_V5.xlsm` to C# for the Ocelis Configurator Blazor application.

## What Has Been Implemented

### Phase 1: Data Extraction & Configuration ✅

#### 1. JSON Configuration File
**Location:** [wwwroot/data/PricingData.json](../src/Ocelis.Configuration.BlazorApp/wwwroot/data/PricingData.json)

This file replaces the Excel cell references with a structured JSON format:

```json
{
  "materials": [
    { "id": 1, "name": "C89x41x1", "multiplier": 1.5 },
    { "id": 2, "name": "C89x41x1,2", "multiplier": 1.8 }
  ],
  "spacingValues": {
    "RD": 2.0,
    "Vestavek": 2.5
  },
  "trussCoefficients": {
    "RD": {
      "Flat300mm": { "3": 0.0, "4": 0.0, ... },
      "Pitched15": { "3": 0.0, "4": 0.0, ... },
      "Pitched35": { "3": 0.0, "4": 0.0, ... },
      "Pitched45": { "3": 0.0, "4": 0.0, ... }
    },
    "Vestavek": {
      "Flat400mm": { "3": 0.0, "4": 0.0, ... }
    }
  }
}
```

**⚠️ ACTION REQUIRED:** You need to extract the actual coefficient values from the Excel file and populate the JSON file. The coefficients are currently set to `0.0` as placeholders.

#### Excel Cell Mapping:
- `C3-C8` → `RD.Flat300mm` coefficients (widths 3m-8m)
- `F3-F8` → `Vestavek.Flat400mm` coefficients (widths 3m-8m)
- `I3-I8` → `RD.Pitched15` coefficients (widths 3m-8m)
- `L3-L8` → `RD.Pitched35` coefficients (widths 3m-8m)
- `O3-O8` → `RD.Pitched45` coefficients (widths 3m-8m)

#### 2. PricingConfiguration Service
**Location:** [Services/PricingConfiguration.cs](../src/Ocelis.Configuration.BlazorApp/Services/PricingConfiguration.cs)

- Loads JSON data at application startup
- Provides methods to retrieve:
  - Material multipliers by ID
  - Spacing values (roztec) by building type
  - Truss coefficients by building type, truss type, and width
- Registered as Singleton in `Program.cs`

### Phase 2: Domain Models & Calculation Service ✅

#### 1. Domain Enums

**BuildingType** ([Domain/BuildingType.cs](../src/Ocelis.Configuration.BlazorApp/Domain/BuildingType.cs))
```csharp
public enum BuildingType
{
    Unknown = 0,
    RD = 1,           // Family house - 2m spacing
    Vestavek = 2      // Commercial - 2.5m spacing
}
```
Maps to VBA variable: `stavba`

**TrussType** ([Domain/TrussType.cs](../src/Ocelis.Configuration.BlazorApp/Domain/TrussType.cs))
```csharp
public enum TrussType
{
    Unknown = 0,
    Flat300mm = 1,    // RD flat roof
    Flat400mm = 2,    // Vestavek flat roof
    Pitched15 = 3,    // 15° pitched roof
    Pitched35 = 4,    // 35° pitched roof
    Pitched45 = 5     // 45° pitched roof
}
```
Maps to VBA variables: `vaznik` and `strecha`

**MaterialType** ([Domain/MaterialType.cs](../src/Ocelis.Configuration.BlazorApp/Domain/MaterialType.cs))
```csharp
public record MaterialType
{
    public int Id { get; init; }
    public string Name { get; init; }
    public decimal Multiplier { get; init; }

    public static readonly MaterialType Standard = new()
        { Id = 1, Name = "C89x41x1", Multiplier = 1.5m };
    public static readonly MaterialType Premium = new()
        { Id = 2, Name = "C89x41x1,2", Multiplier = 1.8m };
}
```
Maps to VBA variable: `material`

#### 2. PricingField Class
**Location:** [Domain/PricingField.cs](../src/Ocelis.Configuration.BlazorApp/Domain/PricingField.cs)

Represents a single field/span in the building:
```csharp
public class PricingField
{
    public decimal Width { get; set; }    // sirka (meters)
    public decimal Length { get; set; }   // delka (meters)
}
```

#### 3. PricingCalculationService
**Location:** [Services/PricingCalculationService.cs](../src/Ocelis.Configuration.BlazorApp/Services/PricingCalculationService.cs)

This is the main calculation engine that replaces the VBA functions.

**Method: CalculateTrussCost**
- **Replaces VBA:** `Function Vazniky(delka, roztec, material, stavba, sirka, vaznik)`
- **Purpose:** Calculate cost of trusses for multiple fields
- **Logic:**
  1. Get spacing (roztec) based on building type
  2. For each field:
     - Validate width ≤ 8m
     - Get coefficient from JSON based on building type, truss type, and width range
     - Calculate: `coefficient × length × spacing × material multiplier`
  3. Sum costs for all fields

**Method: CalculateWallCost**
- **Replaces VBA:** `Function Steny(roztec, vyska, delka_sten, material)`
- **Purpose:** Calculate cost of walls
- **Logic:**
  1. Get spacing (roztec) based on building type
  2. Calculate `roz = (spacing == 2) ? 0.5 : 0.625`
  3. Formula part 1: `(wallLength × 3) + (height × wallLength × spacing) + ((wallLength / 2.5) × height)`
  4. Formula part 2: `+ (√(roz² + (height/4)²) × 4) × (wallLength / 2.5)`
  5. Multiply by material multiplier

**Method: CalculateTotalCost**
- **Replaces VBA:** `Sub m()` orchestration
- **Purpose:** Calculate total cost with breakdown
- **Returns:** `PricingCalculationResult` object containing:
  - `WallCost` - Cost of walls
  - `TrussCost` - Cost of trusses
  - `TotalCost` - Sum of both
  - Metadata (building type, truss type, material type, field count)

## VBA to C# Mapping

### Key Variable Mappings

| VBA Variable | C# Equivalent | Type |
|--------------|---------------|------|
| `stavba` | `BuildingType` enum | RD / Vestavek |
| `vaznik` | `TrussType` enum | Flat300mm, Flat400mm, Pitched15/35/45 |
| `sirka` | `PricingField.Width` | decimal (meters) |
| `delka` | `PricingField.Length` | decimal (meters) |
| `vyska` | `height` parameter | decimal (meters) |
| `delka_sten` | `wallLength` parameter | decimal (meters) |
| `roztec` | `spacing` (from config) | decimal (2.0 or 2.5) |
| `material` | `MaterialType.Multiplier` | decimal (1.5 or 1.8) |
| `vysledek` | `fieldCost` / `result` | decimal |

### Function Mappings

| VBA Function | C# Method | Notes |
|-------------|-----------|-------|
| `Function Vazniky(...)` | `CalculateTrussCost(...)` | Handles single or multiple fields |
| `Function Steny(...)` | `CalculateWallCost(...)` | Direct 1:1 mapping |
| `Sub m()` | `CalculateTotalCost(...)` | Orchestrates both calculations |

### Formula Translations

**Truss Cost (VBA):**
```vba
vysledek = coefficient * delka * roztec
Vazniky = vysledek * material
```

**Truss Cost (C#):**
```csharp
decimal fieldCost = coefficient * field.Length * spacing;
fieldCost *= materialType.Multiplier;
```

**Wall Cost (VBA):**
```vba
vysledek_stena = ((delka_sten * 3 + (vyska * delka_sten * roztec)) + (delka_sten / 2.5) * vyska)
vysledek_stena = vysledek_stena + (Sqr((roz * roz) + (vyska / 4) * (vyska / 4)) * 4) * (delka_sten / 2.5)
Steny = vysledek_stena * material
```

**Wall Cost (C#):**
```csharp
decimal result = (wallLength * 3) +
                (height * wallLength * spacing) +
                ((wallLength / 2.5m) * height);
decimal diagonalComponent = (decimal)Math.Sqrt((double)((roz * roz) + ((height / 4) * (height / 4))));
result += (diagonalComponent * 4) * (wallLength / 2.5m);
result *= materialType.Multiplier;
```

## How to Use the New System

### 1. Update Pricing Data

To change pricing coefficients without redeploying the application:

1. Edit [wwwroot/data/PricingData.json](../src/Ocelis.Configuration.BlazorApp/wwwroot/data/PricingData.json)
2. Update the coefficient values for the appropriate building type, truss type, and width
3. Save the file
4. Restart the application (or implement hot-reload in the future)

### 2. Use in Blazor Components

```csharp
@inject PricingCalculationService PricingService

@code {
    private async Task CalculatePrice()
    {
        var fields = new List<PricingField>
        {
            new PricingField(6.0m, 10.0m),  // 6m wide, 10m long
            new PricingField(5.0m, 8.0m)    // 5m wide, 8m long
        };

        var result = PricingService.CalculateTotalCost(
            fields: fields,
            buildingType: BuildingType.RD,
            trussType: TrussType.Pitched35,
            height: 3.0m,
            wallLength: 24.0m,
            materialType: MaterialType.Standard
        );

        Console.WriteLine($"Wall Cost: {result.WallCost:C}");
        Console.WriteLine($"Truss Cost: {result.TrussCost:C}");
        Console.WriteLine($"Total Cost: {result.TotalCost:C}");
    }
}
```

## Next Steps

### Immediate Actions Required

1. **Extract Excel Data:**
   - Open `Tabulka CN_V5.xlsm`
   - Copy coefficient values from cells C3-C8, F3-F8, I3-I8, L3-L8, O3-O8
   - Update `PricingData.json` with actual values

2. **Test Calculations:**
   - Create test cases comparing Excel calculations with C# calculations
   - Verify formulas produce identical results
   - Test edge cases (width > 8m, zero values, etc.)

### Phase 3: UI Integration (Future Work)

Update [ConfiguratorFormPage.razor](../src/Ocelis.Configuration.BlazorApp/Components/Pages/ConfiguratorFormPage.razor) to:
- Add building type dropdown (RD/Vestavek)
- Expand roof type to include all truss variants
- Add material type selection
- Add height and wall length fields
- Support multiple fields input (dynamic list)
- Replace simple price calculation with `PricingCalculationService`
- Display cost breakdown (walls + trusses)

### Phase 4: Admin Interface (Optional)

Create an admin page for:
- Viewing and editing pricing coefficients
- Upload/download PricingData.json
- Authentication/authorization
- Audit logging of pricing changes

## Benefits of the New System

1. **Data-Driven:** Pricing coefficients stored in JSON, not hard-coded
2. **Maintainable:** Customer can update prices without code changes
3. **Testable:** C# methods can be unit tested
4. **Scalable:** Easy to add new building types, truss types, or material types
5. **Type-Safe:** Strongly-typed enums and models prevent errors
6. **Documented:** Comprehensive XML documentation and comments
7. **Logged:** Detailed logging for debugging and auditing
8. **Cloud-Ready:** Works in Azure App Service without Excel dependencies

## Service Registration

All services are automatically registered in [Program.cs](../src/Ocelis.Configuration.BlazorApp/Program.cs):

```csharp
// Add Pricing services
builder.Services.AddSingleton<PricingConfiguration>();
builder.Services.AddScoped<PricingCalculationService>();

// Load pricing configuration at startup
var pricingConfig = app.Services.GetRequiredService<PricingConfiguration>();
await pricingConfig.LoadAsync();
```

## Files Created

### Domain Models
- [Domain/BuildingType.cs](../src/Ocelis.Configuration.BlazorApp/Domain/BuildingType.cs)
- [Domain/TrussType.cs](../src/Ocelis.Configuration.BlazorApp/Domain/TrussType.cs)
- [Domain/MaterialType.cs](../src/Ocelis.Configuration.BlazorApp/Domain/MaterialType.cs)
- [Domain/PricingField.cs](../src/Ocelis.Configuration.BlazorApp/Domain/PricingField.cs)

### Services
- [Services/PricingConfiguration.cs](../src/Ocelis.Configuration.BlazorApp/Services/PricingConfiguration.cs)
- [Services/PricingCalculationService.cs](../src/Ocelis.Configuration.BlazorApp/Services/PricingCalculationService.cs)

### Configuration
- [wwwroot/data/PricingData.json](../src/Ocelis.Configuration.BlazorApp/wwwroot/data/PricingData.json)

### Documentation
- This file: [docs/VBA_Migration_Guide.md](./VBA_Migration_Guide.md)

## Troubleshooting

### Issue: Application fails to start
**Cause:** PricingData.json not found or invalid JSON
**Solution:** Check file path and JSON syntax

### Issue: Calculations return 0
**Cause:** Coefficients not populated in JSON (still 0.0)
**Solution:** Extract and populate actual values from Excel

### Issue: Width validation fails
**Cause:** Width > 8m
**Solution:** VBA code returns 0 for width > 8m - this is intentional

## Support

For questions or issues with the pricing calculation system, refer to:
- VBA source code in `Tabulka CN_V5.xlsm`
- C# implementation in `Services/PricingCalculationService.cs`
- This migration guide
