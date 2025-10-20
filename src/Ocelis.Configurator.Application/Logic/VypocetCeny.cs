namespace Ocelis.Configurator.Application.Logic;

using Ocelis.Configuration.Domain.Entities;

public class VypocetCeny
{
    private readonly List<VaznikMaterial> _vaznikMaterialy;

    public VypocetCeny(IEnumerable<VaznikMaterial> vaznikMaterialy)
    {
        _vaznikMaterialy = vaznikMaterialy.ToList();
    }
    
    public ZakazkaCena VypoctiCenuZakazky(Zakazka zakazka)
    {
        return new ZakazkaCena();
    }
}