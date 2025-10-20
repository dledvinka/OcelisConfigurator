namespace Ocelis.Configuration.Domain.Tests;

using FluentAssertions;
using Ocelis.Configuration.Domain.Entities;
using Ocelis.Configuration.Domain.Enums;
using Ocelis.Configuration.Domain.Services;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void VypocetCenyTest_Plochy_RD_JednaMistnost_5x8m_CenaSouhlasi()
    {
        var cenikTest = new Cenik(150, 15, 120);
        
        var zakazkaTest = GetDefaultZakazka(cenikTest, StavbaTyp.RodinnyDum, VaznikTyp.Plochy);
        zakazkaTest.PocetVelkychOtvoru = 2;

        var vaznikMaterialy = new List<VaznikMaterial>();
        var cena = new VypocetCenyService(vaznikMaterialy).VypoctiCenuZakazky(zakazkaTest);

        cena.CenaCelkemCzk.Should().Be(344700);
        cena.CenaOcelovaKonstrukceOcelisCzk.Should().Be(231500);
        cena.CenaSilnostennaKonstrukceCzk.Should().Be(78400);
        cena.CenaOplasteniCzk.Should().Be(0);
        cena.CenaMontazNaStavbeCzk.Should().Be(23200);
        cena.CenaManipulacniTechnikaCzk.Should().Be(0);
        cena.CenaSpojovaciMaterialCzk.Should().Be(11600);
    }
    
    [Test]
    public void VypocetCenyTest_SedlovySklon15_RD_JednaMistnost_6x6m_CenaSouhlasi()
    {
        var cenikTest = new Cenik(150, 15, 120);
        
        var zakazkaTest = GetDefaultZakazka(cenikTest, StavbaTyp.RodinnyDum, VaznikTyp.SedlovySklon15);
        zakazkaTest.Delka = Vzdalenost.FromMetry(6);
        zakazkaTest.Sirka = Vzdalenost.FromMetry(6);
        zakazkaTest.PocetVelkychOtvoru = 4;
        zakazkaTest.SvetlaVyskaSten = Vzdalenost.FromMetry(3.2);

        var vaznikMaterialy = new List<VaznikMaterial>();
        var cena = new VypocetCenyService(vaznikMaterialy).VypoctiCenuZakazky(zakazkaTest);

        cena.CenaCelkemCzk.Should().Be(352700);
        cena.CenaOcelovaKonstrukceOcelisCzk.Should().Be(238400);
        cena.CenaSilnostennaKonstrukceCzk.Should().Be(78400);
        cena.CenaOplasteniCzk.Should().Be(0);
        cena.CenaMontazNaStavbeCzk.Should().Be(23900);
        cena.CenaManipulacniTechnikaCzk.Should().Be(0);
        cena.CenaSpojovaciMaterialCzk.Should().Be(12000);
    }
    
    [Test]
    public void VypocetCenyTest_SedlovySklon35_RD_JednaMistnost_6x7m_CenaSouhlasi()
    {
        var cenikTest = new Cenik(150, 15, 120);
        
        var zakazkaTest = GetDefaultZakazka(cenikTest, StavbaTyp.RodinnyDum, VaznikTyp.SedlovySklon15);
        zakazkaTest.Delka = Vzdalenost.FromMetry(6);
        zakazkaTest.Sirka = Vzdalenost.FromMetry(7);
        zakazkaTest.PocetVelkychOtvoru = 4;
        zakazkaTest.SvetlaVyskaSten = Vzdalenost.FromMetry(2.8);

        var vaznikMaterialy = new List<VaznikMaterial>();
        var cena = new VypocetCenyService(vaznikMaterialy).VypoctiCenuZakazky(zakazkaTest);

        cena.CenaCelkemCzk.Should().Be(410800);
        cena.CenaOcelovaKonstrukceOcelisCzk.Should().Be(289000);
        cena.CenaSilnostennaKonstrukceCzk.Should().Be(78400);
        cena.CenaOplasteniCzk.Should().Be(0);
        cena.CenaMontazNaStavbeCzk.Should().Be(28900);
        cena.CenaManipulacniTechnikaCzk.Should().Be(0);
        cena.CenaSpojovaciMaterialCzk.Should().Be(14500);
    }
    
    [Test]
    public void VypocetCenyTest_SedlovySklon45_RD_JednaMistnost_6x7m_CenaSouhlasi()
    {
        var cenikTest = new Cenik(150, 15, 120);
        
        var zakazkaTest = GetDefaultZakazka(cenikTest, StavbaTyp.RodinnyDum, VaznikTyp.SedlovySklon15);
        zakazkaTest.Delka = Vzdalenost.FromMetry(7);
        zakazkaTest.Sirka = Vzdalenost.FromMetry(7);
        zakazkaTest.PocetVelkychOtvoru = 4;
        zakazkaTest.SvetlaVyskaSten = Vzdalenost.FromMetry(2.9);

        var vaznikMaterialy = new List<VaznikMaterial>();
        var cena = new VypocetCenyService(vaznikMaterialy).VypoctiCenuZakazky(zakazkaTest);

        cena.CenaCelkemCzk.Should().Be(502700);
        cena.CenaOcelovaKonstrukceOcelisCzk.Should().Be(368900);
        cena.CenaSilnostennaKonstrukceCzk.Should().Be(78400);
        cena.CenaOplasteniCzk.Should().Be(0);
        cena.CenaMontazNaStavbeCzk.Should().Be(36900);
        cena.CenaManipulacniTechnikaCzk.Should().Be(0);
        cena.CenaSpojovaciMaterialCzk.Should().Be(18500);
    }

    private static Zakazka GetDefaultZakazka(Cenik cenikTest, StavbaTyp stavbaTyp, VaznikTyp vaznikTyp) =>
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
            Cenik = cenikTest
        };
}