namespace Ocelis.Configuration.Domain.Services;

using Ocelis.Configuration.Domain.Entities;

public class VypocetCenyService
{
    private readonly List<VaznikMaterial> _vaznikMaterialy;

    public VypocetCenyService(IEnumerable<VaznikMaterial> vaznikMaterialy)
    {
        _vaznikMaterialy = vaznikMaterialy.ToList();
    }
    
    public ZakazkaCena VypoctiCenuZakazky(Zakazka zakazka)
    {
        return new ZakazkaCena();
    }
}