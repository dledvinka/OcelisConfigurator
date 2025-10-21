namespace Ocelis.Configurator.Application.Cenik;

using Ocelis.Configuration.Domain.Entities;

public class CenikReader
{
    public Cenik GetCenik() => new(150, 15, 120);
}