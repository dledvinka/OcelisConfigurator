namespace Ocelis.Configuration.Domain.Entities;

public record CProfilTyp
{
    private CProfilTyp(string kod) => Kod = kod;

    public string Kod { get; init; }
    
    public static CProfilTyp C89x41x1_0 = new CProfilTyp("C89x41x1");
    public static CProfilTyp C89x41x1_2 = new CProfilTyp("C89x41x1.2");
    public static CProfilTyp C160x41x1 = new CProfilTyp("C160x41x1");
}