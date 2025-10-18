namespace Ocelis.Configuration.Domain.Entities;

using System.Diagnostics;

[DebuggerDisplay("{Gramy} g")]
public sealed class Hmotnost
{
    public double Kilogramy => Gramy / 1000;
    public double Gramy { get; }

    private Hmotnost(double gramy)
    {
        if (gramy < 0)
            throw new ArgumentOutOfRangeException(nameof(gramy));
        Gramy = gramy;
    }

    public static Hmotnost FromGramy(double gramy) => new(gramy);
    public static Hmotnost FromKilogramy(double kilogramy) => new(kilogramy * 1000);
}