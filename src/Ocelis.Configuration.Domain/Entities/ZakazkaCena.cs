namespace Ocelis.Configuration.Domain.Entities;

public class ZakazkaCena
{
    public decimal CenaCelkemCzk => CenaOcelovaKonstrukceOcelisCzk + CenaSilnostennaKonstrukceCzk + CenaOplasteniCzk + CenaMontazNaStavbeCzk + CenaManipulacniTechnikaCzk + CenaSpojovaciMaterialCzk;
    public decimal CenaOcelovaKonstrukceOcelisCzk { get; set; }
    public decimal CenaSilnostennaKonstrukceCzk { get; set; }
    public decimal CenaOplasteniCzk { get; set; }
    public decimal CenaMontazNaStavbeCzk { get; set; }
    public decimal CenaManipulacniTechnikaCzk { get; set; }
    public decimal CenaSpojovaciMaterialCzk { get; set; }
}