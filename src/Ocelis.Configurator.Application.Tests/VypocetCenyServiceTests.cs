namespace Ocelis.Configuration.Domain.Tests;

using FluentAssertions;
using Ocelis.Configuration.Domain.Entities;
using Ocelis.Configuration.Domain.Enums;
using Ocelis.Configurator.Application.Logic;
using Ocelis.Configurator.Application.Materialy;

public class Tests
{
    private readonly Cenik _cenik = new Cenik(150, 15, 120);
    private readonly decimal _decimalValuePrecision = 1000.0m;
    private readonly List<VaznikMaterial>  _vaznikMaterialy = new VaznikMaterialyReader().ReadAsync("Data/Materialy.csv").GetAwaiter().GetResult();
    
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void VypocetCenyTest_Plochy_RD_JednaMistnost_5x8m_CenaSouhlasi()
    {
        var zakazkaTest = GetDefaultZakazka(_cenik, StavbaTyp.RodinnyDum, VaznikTyp.Plochy);
        zakazkaTest.PocetVelkychOtvoru = 2;

        var cena = new VypocetCeny(null, _vaznikMaterialy, _cenik).VypoctiCenuZakazky(zakazkaTest);

        cena.CenaCelkemCzk.Should().BeApproximately(249200, _decimalValuePrecision);
        cena.CenaOcelovaKonstrukceOcelisCzk.Should().BeApproximately(182500, _decimalValuePrecision);
        cena.CenaSilnostennaKonstrukceCzk.Should().BeApproximately(39200, _decimalValuePrecision);
        cena.CenaOplasteniCzk.Should().BeApproximately(0, _decimalValuePrecision);
        cena.CenaMontazNaStavbeCzk.Should().BeApproximately(18300, _decimalValuePrecision);
        cena.CenaManipulacniTechnikaCzk.Should().BeApproximately(0, _decimalValuePrecision);
        cena.CenaSpojovaciMaterialCzk.Should().BeApproximately(9200, _decimalValuePrecision);
    }
    
    [Test]
    public void VypocetCenyTest_SedlovySklon15_RD_JednaMistnost_6x6m_CenaSouhlasi()
    {
        var zakazkaTest = GetDefaultZakazka(_cenik, StavbaTyp.RodinnyDum, VaznikTyp.SedlovySklon15);
        zakazkaTest.Delka = Vzdalenost.FromMetry(6);
        zakazkaTest.Sirka = Vzdalenost.FromMetry(6);
        zakazkaTest.PocetVelkychOtvoru = 4;
        zakazkaTest.SvetlaVyskaSten = Vzdalenost.FromMetry(3.2);
        zakazkaTest.Mistnosti = new List<Mistnost>()
        {
            new Mistnost("A", Vzdalenost.FromMetry(6), Vzdalenost.FromMetry(6))
        };

        var cena = new VypocetCeny(null, _vaznikMaterialy, _cenik).VypoctiCenuZakazky(zakazkaTest);

        cena.CenaCelkemCzk.Should().BeApproximately(352700, _decimalValuePrecision);
        cena.CenaOcelovaKonstrukceOcelisCzk.Should().BeApproximately(238400, _decimalValuePrecision);
        cena.CenaSilnostennaKonstrukceCzk.Should().BeApproximately(78400, _decimalValuePrecision);
        cena.CenaOplasteniCzk.Should().BeApproximately(0, _decimalValuePrecision);
        cena.CenaMontazNaStavbeCzk.Should().BeApproximately(23900, _decimalValuePrecision);
        cena.CenaManipulacniTechnikaCzk.Should().BeApproximately(0, _decimalValuePrecision);
        cena.CenaSpojovaciMaterialCzk.Should().BeApproximately(12000, _decimalValuePrecision);
    }
    
    [Test]
    public void VypocetCenyTest_SedlovySklon35_RD_JednaMistnost_6x7m_CenaSouhlasi()
    {
        var zakazkaTest = GetDefaultZakazka(_cenik, StavbaTyp.RodinnyDum, VaznikTyp.SedlovySklon35);
        zakazkaTest.Delka = Vzdalenost.FromMetry(6);
        zakazkaTest.Sirka = Vzdalenost.FromMetry(7);
        zakazkaTest.PocetVelkychOtvoru = 4;
        zakazkaTest.SvetlaVyskaSten = Vzdalenost.FromMetry(2.8);
        zakazkaTest.Mistnosti = new List<Mistnost>()
        {
            new Mistnost("A", Vzdalenost.FromMetry(6), Vzdalenost.FromMetry(7))
        };

        var cena = new VypocetCeny(null, _vaznikMaterialy, _cenik).VypoctiCenuZakazky(zakazkaTest);

        cena.CenaCelkemCzk.Should().BeApproximately(410800, _decimalValuePrecision);
        cena.CenaOcelovaKonstrukceOcelisCzk.Should().BeApproximately(289000, _decimalValuePrecision);
        cena.CenaSilnostennaKonstrukceCzk.Should().BeApproximately(78400, _decimalValuePrecision);
        cena.CenaOplasteniCzk.Should().BeApproximately(0, _decimalValuePrecision);
        cena.CenaMontazNaStavbeCzk.Should().BeApproximately(28900, _decimalValuePrecision);
        cena.CenaManipulacniTechnikaCzk.Should().BeApproximately(0, _decimalValuePrecision);
        cena.CenaSpojovaciMaterialCzk.Should().BeApproximately(14500, _decimalValuePrecision);
    }
    
    [Test]
    public void VypocetCenyTest_SedlovySklon45_RD_JednaMistnost_6x7m_CenaSouhlasi()
    {
        var zakazkaTest = GetDefaultZakazka(_cenik, StavbaTyp.RodinnyDum, VaznikTyp.SedlovySklon45);
        zakazkaTest.Delka = Vzdalenost.FromMetry(7);
        zakazkaTest.Sirka = Vzdalenost.FromMetry(7);
        zakazkaTest.PocetVelkychOtvoru = 4;
        zakazkaTest.SvetlaVyskaSten = Vzdalenost.FromMetry(2.9);
        zakazkaTest.Mistnosti = new List<Mistnost>()
        {
            new Mistnost("A", Vzdalenost.FromMetry(7), Vzdalenost.FromMetry(7))
        };

        var cena = new VypocetCeny(null, _vaznikMaterialy, _cenik).VypoctiCenuZakazky(zakazkaTest);

        cena.CenaCelkemCzk.Should().BeApproximately(502700, _decimalValuePrecision);
        cena.CenaOcelovaKonstrukceOcelisCzk.Should().BeApproximately(368900, _decimalValuePrecision);
        cena.CenaSilnostennaKonstrukceCzk.Should().BeApproximately(78400, _decimalValuePrecision);
        cena.CenaOplasteniCzk.Should().BeApproximately(0, _decimalValuePrecision);
        cena.CenaMontazNaStavbeCzk.Should().BeApproximately(36900, _decimalValuePrecision);
        cena.CenaManipulacniTechnikaCzk.Should().BeApproximately(0, _decimalValuePrecision);
        cena.CenaSpojovaciMaterialCzk.Should().BeApproximately(18500, _decimalValuePrecision);
    }

    private static Zakazka GetDefaultZakazka(Cenik cenik, StavbaTyp stavbaTyp, VaznikTyp vaznikTyp) =>
        new()
        {
            StavbaTyp = stavbaTyp,
            VaznikTyp = vaznikTyp,
            PocetVelkychOtvoru = 4,
            CProfilTyp = CProfilTyp.C89x41x1_0,
            SvetlaVyskaSten = Vzdalenost.FromMetry(3),
            Sirka = Vzdalenost.FromMetry(5),
            Delka = Vzdalenost.FromMetry(8),
            Mistnosti = new List<Mistnost>()
            {
                new Mistnost("A", Vzdalenost.FromMetry(5), Vzdalenost.FromMetry(8))
            },
            Cenik = cenik
        };
}