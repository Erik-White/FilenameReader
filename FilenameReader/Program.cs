using FilenameReader.Cli;
using FilenameReader.Core;
using FilenameReader.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FilenameReaderCli;

public class Program
{
    protected Program() { }

    public static async Task Main(string[] args)
    {
        var serviceProvider = BuildServiceProvider();
        var logger = serviceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger<Program>();

        var textSearcher = serviceProvider.GetRequiredService<ITextSearcher>();
        var filePath = new FilePath(args.FirstOrDefault() ?? string.Empty);

        logger.LogInformation("Counting filename instances in the contents of the file {filepath}", filePath.FullPath);

        var result = await textSearcher.CountFileContentsAsync(filePath, filePath.Filename);

        result.Switch(
            count => logger.LogInformation("Filename count: {count}", count),
            validationFailure => logger.LogError(
                "The file path or name was not valid: {message}", string.Join(Environment.NewLine, validationFailure.ErrorMessages)),
            _ => logger.LogError("The file could not be found."),
            error => logger.LogError(error.Value!, "An unexpected error occurred: {message}", error.Value.Message)
        );
    }

    private static IServiceProvider BuildServiceProvider()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddCliServices();
        serviceCollection.AddInfrastructureServices();

        return serviceCollection.BuildServiceProvider();
    }
}