using FilenameReader.Cli;
using FilenameReader.Core;
using FilenameReader.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FilenameReaderCli;

public class Program
{
    protected Program() { }

    public static void Main(string[] args)
    {
        var serviceProvider = BuildServiceProvider();
        var logger = serviceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger<Program>();

        var fileParser = serviceProvider.GetRequiredService<IFileParser>();
        var filePath = new FilePath(args.FirstOrDefault() ?? string.Empty);

        logger.LogInformation("Counting filename instances in the contents of the file {filepath}", filePath.FullPath);

        try
        {
            var count = fileParser.CountFileContents(filePath);

            logger.LogInformation("Filename count: {count}", count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred: {message}", ex.Message);
        }
    }

    private static IServiceProvider BuildServiceProvider()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddCliServices();
        serviceCollection.AddInfrastructureServices();

        return serviceCollection.BuildServiceProvider();
    }
}