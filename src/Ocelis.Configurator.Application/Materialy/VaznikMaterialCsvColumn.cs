namespace Ocelis.Configurator.Application.Materialy;

using CsvHelper.Configuration.Attributes;

public class VaznikMaterialCsvColumn
{
    [Index(0)]
    public string StavbaTyp { get; set; }
    [Index(1)]
    public string VaznikyTyp { get; set; }
    [Index(2)]
    public double DelkaMinMetry { get; set; }
    [Index(3)]
    public double DelkaMaxMetry { get; set; }
    [Index(4)]
    public string Kod { get; set; }
    [Index(5)]
    public double HmotnostKg { get; set; }
}