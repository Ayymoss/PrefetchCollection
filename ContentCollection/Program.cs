using ContentCollection.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ContentCollection;

public class Program
{
    public static async Task Main()
    {
        var serviceCollection = BuildServices();
        await BuildApp(serviceCollection);
    }

    private static async Task BuildApp(IServiceCollection serviceCollection)
    {
        var app = serviceCollection.BuildServiceProvider();
        app.GetRequiredService<ScheduledService>().Setup();
        await app.GetRequiredService<AppEntry>().Run();
    }

    private static IServiceCollection BuildServices()
    {
        var serviceCollection = new ServiceCollection()
            .AddSingleton<CacheService>()
            .AddSingleton<ScheduledService>()
            .AddSingleton<AppEntry>();
        
        return serviceCollection;
    }
}
