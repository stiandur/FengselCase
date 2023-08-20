using Fengselsadministrasjon.Domain.Entities;
using Fengselsadministrasjon.Domain.Integrations;
using Fengselsadministrasjon.Infrastructure.Mappers;

namespace Fengselsadministrasjon.Infrastructure.FangedataIntegration;

public class FangedataGateway : IFangedataGateway
{
    private readonly FangedataHttpClient _fangedataHttpClient;

    public FangedataGateway(FangedataHttpClient fangedataHttpClient)
    {
        _fangedataHttpClient = fangedataHttpClient;
    }

    public async Task<List<(string, Fange)>> GetFanger()
    {
        var fanger = await _fangedataHttpClient.GetFanger();

        return fanger.Select(x => (x.Cellenummer, FangeMapper.Map(x))).ToList();
    }
}
