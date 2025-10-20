namespace Ocelis.Configurator.Application.Materialy;

using System.Globalization;
using CsvHelper;
using Ocelis.Configuration.Domain.Entities;
using Ocelis.Configuration.Domain.Enums;

public class VaznikMaterialyReader
{
    public List<VaznikMaterial> Read(string filePath)
    {
        using var reader = new StreamReader(filePath);
        var readerConfiguration = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.GetCultureInfo("cs-CZ"));
        readerConfiguration.HasHeaderRecord = true;
        readerConfiguration.Delimiter = ";";
        using var csv = new CsvReader(reader, readerConfiguration, false);
        var csvRows = csv.GetRecords<VaznikMaterialCsvRow>();

        var vaznikMaterialy = csvRows.Select(x => new VaznikMaterial(Enum.Parse<StavbaTyp>(x.StavbaTyp), Enum.Parse<VaznikTyp>(x.VaznikyTyp),
                                                      Vzdalenost.FromMetry(x.SirkaMinMetry), Vzdalenost.FromMetry(x.SirkaMaxMetry), x.Kod,
                                                      Hmotnost.FromKilogramy(x.HmotnostKg))).ToList();

        return vaznikMaterialy;
    }
}