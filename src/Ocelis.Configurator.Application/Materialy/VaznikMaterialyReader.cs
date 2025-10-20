namespace Ocelis.Configurator.Application.Materialy;

using System.Globalization;
using CsvHelper;
using Ocelis.Configuration.Domain.Entities;

public class VaznikMaterialyReader
{
    public List<VaznikMaterial> Read(string filePath)
    {
        using var reader = new StreamReader(filePath);
        var readerConfiguration = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.GetCultureInfo("cs-CZ"));
        readerConfiguration.HasHeaderRecord = true;
        readerConfiguration.Delimiter = ";";
        using var csv = new CsvReader(reader, readerConfiguration, false);
        var records = csv.GetRecords<VaznikMaterialCsvColumn>().ToList();
        
        return new List<VaznikMaterial>();
    }
}