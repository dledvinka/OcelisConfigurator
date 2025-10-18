namespace Ocelis.Configuration.Domain.Entities;

using Ocelis.Configuration.Domain.Enums;

public class Zakazka
{
    public StavbaTyp StavbaTyp { get; set; }
    public VaznikTyp VaznikTyp { get; set; }
    public int PocetVelkychOtvoru { get; set; }
    public CProfilTyp CProfilTyp { get; set; }
    public Vzdalenost SvetlaVyskaSten { get; set; }
    public Vzdalenost Delka { get; set; }
    public Vzdalenost Sirka { get; set; }
    public List<Mistnost> Mistnosti { get; set; }
    public Cenik Cenik { get; set; }
}