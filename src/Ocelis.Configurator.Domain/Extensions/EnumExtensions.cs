using Ocelis.Configuration.Domain.Enums;

namespace Ocelis.Configuration.Domain.Extensions;

public static class EnumExtensions
{
    /// <summary>
    /// Gets the Czech description for StavbaTyp enum values.
    /// </summary>
    public static string ToCzechDescription(this StavbaTyp stavbaTyp)
    {
        return stavbaTyp switch
        {
            StavbaTyp.RodinnyDum => "Rodinný dům",
            StavbaTyp.Vestavek => "Vestavek",
            _ => stavbaTyp.ToString()
        };
    }

    /// <summary>
    /// Gets the Czech description for VaznikTyp enum values.
    /// </summary>
    public static string ToCzechDescription(this VaznikTyp vaznikTyp)
    {
        return vaznikTyp switch
        {
            VaznikTyp.Plochy => "Plochý",
            VaznikTyp.SedlovySklon15 => "Sedlová sklon 15°",
            VaznikTyp.SedlovySklon35 => "Sedlová sklon 35°",
            VaznikTyp.SedlovySklon45 => "Sedlová sklon 45°",
            _ => vaznikTyp.ToString()
        };
    }

    /// <summary>
    /// Gets all StavbaTyp values with their Czech descriptions for dropdown lists.
    /// </summary>
    public static IEnumerable<(StavbaTyp Value, string Description)> GetStavbaTypItems()
    {
        yield return (StavbaTyp.RodinnyDum, StavbaTyp.RodinnyDum.ToCzechDescription());
        yield return (StavbaTyp.Vestavek, StavbaTyp.Vestavek.ToCzechDescription());
    }

    /// <summary>
    /// Gets all VaznikTyp values with their Czech descriptions for dropdown lists.
    /// </summary>
    public static IEnumerable<(VaznikTyp Value, string Description)> GetVaznikTypItems()
    {
        yield return (VaznikTyp.Plochy, VaznikTyp.Plochy.ToCzechDescription());
        yield return (VaznikTyp.SedlovySklon15, VaznikTyp.SedlovySklon15.ToCzechDescription());
        yield return (VaznikTyp.SedlovySklon35, VaznikTyp.SedlovySklon35.ToCzechDescription());
        yield return (VaznikTyp.SedlovySklon45, VaznikTyp.SedlovySklon45.ToCzechDescription());
    }
}
