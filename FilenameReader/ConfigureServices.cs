using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FilenameReader.Cli;

public static class ConfigureServices
{
    public static IServiceCollection AddCliServices(this IServiceCollection services)
    {
        var minimumLogLevel = LogLevel.Information;

#if DEBUG
        minimumLogLevel = LogLevel.Debug;
#endif

        services
            .AddLogging(options =>
            {
                options.SetMinimumLevel(minimumLogLevel);
                options.AddConsole();
            });

        return services;
    }
}