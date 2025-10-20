namespace Ocelis.Configuration.Domain.Entities;

using Ocelis.Configuration.Domain.Enums;

public record VaznikMaterial(StavbaTyp StavbaTyp, VaznikTyp VaznikTyp, Vzdalenost SirkaMin, Vzdalenost SirkaMax, string Kod, Hmotnost Hmotnost);