using Fengselsadministrasjon.Domain.Entities;

namespace Fengselsadministrasjon.Domain.Integrations;

public interface IFangedataGateway
{
    Task<List<(string cellenummer, Fange fange)>> GetFanger();
}
