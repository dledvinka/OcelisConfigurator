namespace Ocelis.Configuration.Domain.Tests;

using Ocelis.Configuration.Domain.Entities;
using Ocelis.Configuration.Domain.Enums;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        var cenikTest = new Cenik(150, 15, 120);
        
        var zakazkaTest = new Zakazka()
        {
            StavbaTyp = StavbaTyp.RodinnyDum,
            VaznikTyp = VaznikTyp.Plochy,
            PocetVelkychOtvoru = 5,
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
}