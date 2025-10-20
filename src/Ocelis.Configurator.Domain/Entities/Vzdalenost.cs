namespace Ocelis.Configuration.Domain.Entities;

using System.Diagnostics;

[DebuggerDisplay("{Milimetry} mm")]
public sealed class Vzdalenost : IComparable<Vzdalenost>, IEquatable<Vzdalenost>
{
    public double Centimetry => Milimetry / 10;
    public double Metry => Milimetry / 1000;

    public double Milimetry { get; }

    private Vzdalenost(double milimetry)
    {
        if (milimetry < 0)
            throw new ArgumentOutOfRangeException(nameof(milimetry), "Vzdálenost nemůže být záporná.");

        Milimetry = milimetry;
    }

    public int CompareTo(Vzdalenost? other) => Milimetry.CompareTo(other?.Milimetry ?? 0.0d);

    public bool Equals(Vzdalenost? other) => other is not null && Milimetry.Equals(other.Milimetry);

    public static Vzdalenost FromMilimetry(double milimetry) => new(milimetry);

    public static Vzdalenost FromCentimetry(double centimetry) => new(centimetry * 10);

    public static Vzdalenost FromMetry(double metry) => new(metry * 1000);

    public static Vzdalenost operator *(double multiplier, Vzdalenost vzdalenost) => FromMilimetry(multiplier * vzdalenost.Milimetry);

    public static double operator /(Vzdalenost a, Vzdalenost b) => a.Milimetry / b.Milimetry;

    public static double operator /(Vzdalenost a, double b) => a.Milimetry / b;

    public static Vzdalenost operator -(Vzdalenost a, Vzdalenost b) => FromMilimetry(a.Milimetry - b.Milimetry);

    public static Vzdalenost operator +(Vzdalenost a, Vzdalenost b) => FromMilimetry(a.Milimetry + b.Milimetry);

    private static int Compare(Vzdalenost first, Vzdalenost second) => (first?.Milimetry ?? 0).CompareTo(second?.Milimetry ?? 0);

    public static bool operator <(Vzdalenost first, Vzdalenost second) => Compare(first, second) < 0;

    public static bool operator >(Vzdalenost first, Vzdalenost second) => Compare(first, second) > 0;

    public static bool operator <=(Vzdalenost first, Vzdalenost second) => Compare(first, second) <= 0;

    public static bool operator >=(Vzdalenost first, Vzdalenost second) => Compare(first, second) >= 0;

    public override bool Equals(object? obj) => obj is Vzdalenost other && Equals(other);

    public override int GetHashCode() => Milimetry.GetHashCode();

    public static bool operator ==(Vzdalenost? left, Vzdalenost? right) => Equals(left, right);

    public static bool operator !=(Vzdalenost? left, Vzdalenost? right) => !Equals(left, right);
}