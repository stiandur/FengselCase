using Fengselsadministrasjon.Infrastructure.DTOs;
using Newtonsoft.Json;

namespace Fengselsadministrasjon.Infrastructure.FangedataIntegration;

public class FangedataHttpClient
{
    private readonly HttpClient _httpClient;

    public FangedataHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<FangeDto>> GetFanger()
    {
        var response = await _httpClient.GetAsync("fanger");
        var responseData = JsonConvert.DeserializeObject<FangerDto>(await response.Content.ReadAsStringAsync());
        return responseData?.Fanger ?? new List<FangeDto>();
    }
}
