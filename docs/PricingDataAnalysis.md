Part 2: Excel-Based Price Calculation
Current Problem:
Your pricing formula is too simple:
_priceEstimate = _model.DimensionA * _model.DimensionB * _model.DimensionC * _model.RoofType.Id / 1000;
Solution Options for Excel-Based Pricing:
🎯 Recommended Approach: Convert Excel to JSON
Why This Approach?
✅ Best Performance - No Excel parsing at runtime
✅ No External Dependencies - Pure .NET, no Excel libraries
✅ Works on Azure - JSON is just a static file
✅ Easy to Update - Edit Excel, convert to JSON, redeploy
✅ Type-Safe - Strong typing with C# models
Implementation Steps:
Step 1: Design Your Excel Structure
Option A: Lookup Table (if you have predefined dimension ranges)
| Min A | Max A | Min B | Max B | Min C | Max C | Roof Type | Price per Unit |
|-------|-------|-------|-------|-------|-------|-----------|----------------|
| 1000  | 5000  | 1000  | 3000  | 1000  | 2000  | Flat      | 150            |
| 1000  | 5000  | 1000  | 3000  | 1000  | 2000  | Pointy    | 180            |
| 5000  | 10000 | 3000  | 6000  | 2000  | 4000  | Flat      | 140            |
Option B: Formula-Based (if you have a complex pricing formula)
{
  "basePrice": 100,
  "roofTypeMultipliers": {
    "Flat": 1.0,
    "Pointy": 1.2
  },
  "dimensionRanges": [
    {
      "minVolume": 0,
      "maxVolume": 1000000,
      "pricePerCubicMM": 0.0001
    },
    {
      "minVolume": 1000000,
      "maxVolume": 5000000,
      "pricePerCubicMM": 0.00008
    }
  ]
}
Step 2: Convert Excel to JSON
Tool Option 1: Online Converter
Use https://www.convertcsv.com/csv-to-json.htm
Export Excel as CSV
Convert CSV to JSON
Clean up the JSON structure
Tool Option 2: Excel to JSON Script (PowerShell)
# Save your Excel as CSV first, then:
$csv = Import-Csv "pricing.csv"
$json = $csv | ConvertTo-Json
$json | Out-File "pricing.json"
Tool Option 3: Python Script
import pandas as pd
import json

df = pd.read_excel('pricing.xlsx')
json_data = df.to_dict('records')
with open('pricing.json', 'w') as f:
    json.dump(json_data, f, indent=2)
Step 3: Add JSON File to Your Project
Place the JSON file in your wwwroot or create a Data folder:
src/Ocelis.Configuration.BlazorApp/
├── wwwroot/
│   └── data/
│       └── pricing.json    ← Add here (publicly accessible)
OR
├── Data/
│   └── pricing.json        ← Add here (embedded resource)
Step 4: Create C# Models
// src/Ocelis.Configuration.BlazorApp/Domain/PricingRule.cs
namespace Ocelis.Configuration.BlazorApp.Domain;

public class PricingRule
{
    public decimal MinA { get; set; }
    public decimal MaxA { get; set; }
    public decimal MinB { get; set; }
    public decimal MaxB { get; set; }
    public decimal MinC { get; set; }
    public decimal MaxC { get; set; }
    public string RoofType { get; set; } = string.Empty;
    public decimal PricePerUnit { get; set; }
}

public class PricingData
{
    public List<PricingRule> Rules { get; set; } = new();
}
Step 5: Create Pricing Service
// src/Ocelis.Configuration.BlazorApp/Services/PricingService.cs
using System.Text.Json;
using Ocelis.Configuration.BlazorApp.Domain;

namespace Ocelis.Configuration.BlazorApp.Services;

public interface IPricingService
{
    Task<decimal> CalculatePriceAsync(decimal dimA, decimal dimB, decimal dimC, string roofType);
}

public class PricingService : IPricingService
{
    private readonly HttpClient _httpClient;
    private PricingData? _cachedPricingData;

    public PricingService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<decimal> CalculatePriceAsync(decimal dimA, decimal dimB, decimal dimC, string roofType)
    {
        // Load pricing data (cached after first load)
        _cachedPricingData ??= await LoadPricingDataAsync();

        // Find matching rule
        var rule = _cachedPricingData.Rules.FirstOrDefault(r =>
            dimA >= r.MinA && dimA <= r.MaxA &&
            dimB >= r.MinB && dimB <= r.MaxB &&
            dimC >= r.MinC && dimC <= r.MaxC &&
            r.RoofType.Equals(roofType, StringComparison.OrdinalIgnoreCase));

        if (rule == null)
        {
            // Fallback or throw exception
            return CalculateFallbackPrice(dimA, dimB, dimC, roofType);
        }

        // Calculate price based on rule
        var volume = dimA * dimB * dimC;
        return volume * rule.PricePerUnit / 1000000; // Convert to m³
    }

    private async Task<PricingData> LoadPricingDataAsync()
    {
        var json = await _httpClient.GetStringAsync("data/pricing.json");
        return JsonSerializer.Deserialize<PricingData>(json) 
            ?? throw new InvalidOperationException("Failed to load pricing data");
    }

    private decimal CalculateFallbackPrice(decimal dimA, decimal dimB, decimal dimC, string roofType)
    {
        // Your current simple formula as fallback
        var roofMultiplier = roofType == "Flat" ? 1.0m : 1.2m;
        return dimA * dimB * dimC * roofMultiplier / 1000;
    }
}
Step 6: Register Service in Program.cs
// Add to Program.cs
builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) 
});
builder.Services.AddScoped<IPricingService, PricingService>();
Step 7: Update ConfiguratorFormPage
@inject IPricingService PricingService

private async Task OnInputChangedAsync()
{
    _priceEstimate = await PricingService.CalculatePriceAsync(
        _model.DimensionA, 
        _model.DimensionB, 
        _model.DimensionC, 
        _model.RoofType.DisplayText);
}
🔄 Alternative Approach: Direct Excel Reading (NOT Recommended for Azure)
If you absolutely need to read Excel directly:
Using EPPlus or ClosedXML
<!-- Add to .csproj -->
<PackageReference Include="EPPlus" Version="7.*" />
<!-- OR -->
<PackageReference Include="ClosedXML" Version="0.102.*" />
Problems with this approach:
❌ Slower performance (parsing Excel every time)
❌ Larger dependencies (~5-10 MB)
❌ More complex deployment
❌ Need to upload Excel file to Azure storage
⚠️ Requires proper licensing (EPPlus non-commercial license)
Where to store Excel on Azure:
Azure Blob Storage (recommended if using this approach)
wwwroot folder (not ideal for large files)
Azure File Share
🎯 My Strong Recommendation:
Use the JSON Approach because:
✅ Works perfectly with your current Azure deployment
✅ No changes needed to GitHub Actions
✅ Fast and efficient
✅ Easy to maintain - Just convert Excel → JSON when pricing changes
✅ No additional Azure services needed
✅ No licensing concerns
Workflow for Price Updates:
Update your Excel file with new prices
Convert Excel to JSON (using any method above)
Replace wwwroot/data/pricing.json
Commit and push to GitHub
GitHub Actions automatically deploys the update
Done! ✅
📝 Next Steps:
Would you like me to:
Implement the JSON-based pricing system for you?
Show you the exact Excel structure you should use?
Create a conversion script to automate Excel → JSON?
Implement the Excel reading approach (if you have specific reasons)?
Let me know which approach you prefer, and I can implement it for you! 🚀