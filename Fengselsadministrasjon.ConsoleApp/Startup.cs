using Fengselsadministrasjon.ConsoleApp.Services;
using Fengselsadministrasjon.Domain.Integrations;
using Fengselsadministrasjon.Infrastructure.Config;
using Fengselsadministrasjon.Infrastructure.FangedataIntegration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Text;

namespace Fengselsadministrasjon.ConsoleApp;

public class Startup
{
    IConfigurationRoot Configuration { get; }

    public Startup()
    {
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false);

        Configuration = builder.Build();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        var fangedataApiConfiguration = new FangedataApiConfiguration();
        Configuration.GetSection("FangedataApiConfiguration").Bind(fangedataApiConfiguration);

        services.AddTransient<IFangedataGateway, FangedataGateway>();
        services.AddTransient<FengselsadministrasjonService>();

        services.AddHttpClient<FangedataHttpClient>(client =>
        {
            client.BaseAddress = new Uri(fangedataApiConfiguration.BaseUri);
            client.Timeout = TimeSpan.FromSeconds(fangedataApiConfiguration.Timeout);

            var byteArray = Encoding.ASCII.GetBytes($"{fangedataApiConfiguration.Username}:{fangedataApiConfiguration.Password}");
            var header = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            client.DefaultRequestHeaders.Authorization = header;
        });
    }
}
