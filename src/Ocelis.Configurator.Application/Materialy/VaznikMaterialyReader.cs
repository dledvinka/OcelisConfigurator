namespace Ocelis.Configurator.Application.Materialy;

using System.Globalization;
using CsvHelper;
using Ocelis.Configuration.Domain.Entities;
using Ocelis.Configuration.Domain.Enums;

public class VaznikMaterialyReader
{
    public async Task<List<VaznikMaterial>> ReadAsync(string csvFilePath)
    {
        using var reader = new StreamReader(csvFilePath);
        return await ReadAsync(reader);
    }

    public async Task<List<VaznikMaterial>> ReadAsync(Stream cvsFileStream)
    {
        using var reader = new StreamReader(cvsFileStream);
        return await ReadAsync(reader);
    }

    private async Task<List<VaznikMaterial>> ReadAsync(TextReader cvsFileTextReader)
    {
        var readerConfiguration = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.GetCultureInfo("cs-CZ"));
        readerConfiguration.HasHeaderRecord = true;
        readerConfiguration.Delimiter = ";";
        using var csv = new CsvReader(cvsFileTextReader, readerConfiguration, false);
        var csvRows = csv.GetRecordsAsync<VaznikMaterialCsvRow>();

        var vaznikMaterialy = await csvRows.Select(x => new VaznikMaterial(Enum.Parse<StavbaTyp>(x.StavbaTyp), Enum.Parse<VaznikTyp>(x.VaznikyTyp),
                                                                           Vzdalenost.FromMetry(x.SirkaMinMetry), Vzdalenost.FromMetry(x.SirkaMaxMetry), x.Kod,
                                                                           Hmotnost.FromKilogramy(x.HmotnostKg))).ToListAsync();

        return vaznikMaterialy;
    }
}