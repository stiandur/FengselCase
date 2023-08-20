using Fengselsadministrasjon.ConsoleApp;
using Fengselsadministrasjon.ConsoleApp.Services;
using Microsoft.Extensions.DependencyInjection;

IServiceCollection services = new ServiceCollection();
Startup startup = new();
startup.ConfigureServices(services);
IServiceProvider serviceProvider = services.BuildServiceProvider();

var fengselsadministrasjonService = serviceProvider.GetService<FengselsadministrasjonService>();
await fengselsadministrasjonService.Run();
