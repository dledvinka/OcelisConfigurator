namespace Ocelis.Configuration.Domain.Entities;

using Ocelis.Configuration.Domain.Enums;

public record VaznikMaterial(StavbaTyp StavbaTyp, VaznikTyp VaznikTyp, Vzdalenost DelkaMin, Vzdalenost DelkaMax, string Kod, Hmotnost Hmotnost);