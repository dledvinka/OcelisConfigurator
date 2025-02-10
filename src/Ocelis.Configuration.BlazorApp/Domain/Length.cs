namespace Ocelis.Configuration.BlazorApp.Domain;

public class Length
{
    public Length(decimal millimeters)
    {
        Millimeters = millimeters;
    }

    public decimal Millimeters { get; }
}
