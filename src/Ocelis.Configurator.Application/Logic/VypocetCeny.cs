namespace Ocelis.Configurator.Application.Logic;

using Ocelis.Configuration.Domain.Entities;
using Ocelis.Configuration.Domain.Enums;

public class VypocetCeny
{
    private readonly Cenik _cenik;
    private readonly List<VaznikMaterial> _vaznikMaterialy;

    public VypocetCeny(IEnumerable<VaznikMaterial> vaznikMaterialy, Cenik cenik)
    {
        _cenik = cenik;
        _vaznikMaterialy = vaznikMaterialy.ToList();
    }
    
    public ZakazkaCena VypoctiCenuZakazky(Zakazka zakazka)
    {
        var koeficientRoztece = GetRoztec(zakazka.StavbaTyp);
        var koeficientMaterialu = GetMaterial(zakazka.CProfilTyp);
        var delkaSten = 2 * zakazka.Sirka + 2 * zakazka.Delka;
        var stenyHmotnostKg = GetStenyHmotnostKg(koeficientRoztece, zakazka.SvetlaVyskaSten.Metry, delkaSten.Metry, koeficientMaterialu);

        var vaznikyHmotnostKg = 0d;

        foreach (var mistnost in zakazka.Mistnosti)
        {
            vaznikyHmotnostKg += GetVaznikyHmotnostKg(mistnost.Delka, koeficientRoztece, koeficientMaterialu, zakazka.StavbaTyp, mistnost.Sirka, zakazka.VaznikTyp);
        }

        var silnoStennaOcelHmotnostKg = zakazka.PocetVelkychOtvoru * 8 * 20.4;
        
        var celkovaHmotnostKg = stenyHmotnostKg + vaznikyHmotnostKg + silnoStennaOcelHmotnostKg;
        var cenaOcelovaKonstrukceOcelisCzk = (decimal)celkovaHmotnostKg * _cenik.CenaTenkostennaOcelZaKg;

        return new ZakazkaCena()
        {
            CenaOcelovaKonstrukceOcelisCzk = cenaOcelovaKonstrukceOcelisCzk,
            CenaSilnostennaKonstrukceCzk = (decimal)silnoStennaOcelHmotnostKg * _cenik.CenaSilnostennaOcelZaKgCzk,
            CenaMontazNaStavbeCzk = (decimal)celkovaHmotnostKg * _cenik.CenaMontazZaKgCzk,
            CenaSpojovaciMaterialCzk = cenaOcelovaKonstrukceOcelisCzk * 0.05m,
            CenaOplasteniCzk = 0,
            CenaManipulacniTechnikaCzk = 0,
        };
    }

    private double GetVaznikyHmotnostKg(Vzdalenost mistnostDelka, double koeficientRoztece, double koeficientMaterialu, StavbaTyp zakazkaStavbaTyp,
                              Vzdalenost mistnostSirka, VaznikTyp zakazkaVaznikTyp)
    {
        var vaznikMaterial = _vaznikMaterialy.FirstOrDefault(x => x.StavbaTyp == zakazkaStavbaTyp && x.VaznikTyp == zakazkaVaznikTyp && mistnostSirka >= x.SirkaMin && mistnostSirka <= x.SirkaMax);
        
        if (vaznikMaterial == null)
            throw new ArgumentOutOfRangeException(nameof(zakazkaStavbaTyp), zakazkaStavbaTyp, null);
        
        var vaznikyHmotnostKg = vaznikMaterial.Hmotnost.Kilogramy * mistnostDelka.Metry * koeficientRoztece * koeficientMaterialu;
        
        return vaznikyHmotnostKg;
    }

    private double GetStenyHmotnostKg(double koeficientRoztece, double svetlaVyskaStenMetry, double delkaStenMetry, double koeficientMaterialu)
    {
        // takto je to ve zdrojovém VBA scriptu
        var roz = koeficientRoztece == 2 ? 0.5 : 0.625;
        
        // vysledek_stena = ((delka_sten * 3 + (vyska * delka_sten * roztec)) + (delka_sten / 2.5) * vyska)
        // Range("B2").Value = (Sqr((roz * roz) + (vyska / 4) * (vyska / 4)) * 4 * 1.5) * (delka_sten / 2.5)
        // (
        // Sqr(
        // (roz * roz) + (vyska / 4) * (vyska / 4)
        // )
        // * 4 * 1.5
        // ) * (delka_sten / 2.5)
        // vysledek_stena = vysledek_stena + (Sqr((roz * roz) + (vyska / 4) * (vyska / 4)) * 4) * (delka_sten / 2.5)
        //
        //
        // Range("B1").Value = vysledek_stena * material
        // Steny = vysledek_stena * material

        var vysledekStena = 3 * delkaStenMetry + koeficientRoztece * svetlaVyskaStenMetry * delkaStenMetry + delkaStenMetry / 2.5 * svetlaVyskaStenMetry;
        var rangeB2 = Math.Sqrt(roz * roz + svetlaVyskaStenMetry / 4 * (svetlaVyskaStenMetry / 4)) * 4 * 1.5 * (delkaStenMetry / 2.5);
        vysledekStena += Math.Sqrt(roz * roz + svetlaVyskaStenMetry / 4 * (svetlaVyskaStenMetry / 4)) * 4 * (delkaStenMetry / 2.5);

        var vysledekSteny = koeficientMaterialu * vysledekStena;

        return vysledekSteny;
    }

    private double GetMaterial(CProfilTyp zakazkaCProfilTyp)
    {
        if (zakazkaCProfilTyp == CProfilTyp.C89x41x1_0)
            return 1.5;
        else if (zakazkaCProfilTyp == CProfilTyp.C89x41x1_2)
            return 1.8;
        else
            throw new ArgumentOutOfRangeException(nameof(zakazkaCProfilTyp), zakazkaCProfilTyp, null);
    }

    private double GetRoztec(StavbaTyp stavbaTyp)
    {
        // nerozlišeno v původním kódu VBA, pro oba případy vrací 2
        return stavbaTyp switch
        {
            StavbaTyp.RodinnyDum or StavbaTyp.Vestavek => 2,
            StavbaTyp.Neznamy => throw new ArgumentOutOfRangeException(nameof(stavbaTyp), stavbaTyp, null),
            _ => throw new ArgumentOutOfRangeException(nameof(stavbaTyp), stavbaTyp, null)
        };
    }
}