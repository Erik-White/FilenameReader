using FilenameReader.Cli;
using FilenameReader.Core;
using FilenameReader.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FilenameReaderCli;

public class Program
{
    public enum ReturnCode
    {
        Success = 0,
        Error = -1
    }

    protected Program() { }

    public static async Task<int> Main(string[] args)
    {
        var returnCode = ReturnCode.Success;
        var serviceProvider = BuildServiceProvider();
        var logger = serviceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger<Program>();

        try
        {

            var textSearcher = serviceProvider.GetRequiredService<IFileTextSearcher>();
            var filePath = new FilePath(args.FirstOrDefault() ?? string.Empty);

            logger.LogInformation("Counting filename {filename} instances in the contents of the file {filepath}", filePath.Filename, filePath.FullPath);

            var result = await textSearcher
                .CountFileContentsAsync(filePath, filePath.Filename, progress: GetProgress(logger))
                .ConfigureAwait(false);

            result.Switch(
                count => logger.LogInformation("Filename count: {count}", count),
                validationFailure => logger.LogError(
                    "The file path or name was not valid: {message}", string.Join(Environment.NewLine, validationFailure.ErrorMessages)),
                _ => logger.LogError("The file could not be found."),
                error => logger.LogError(error.Value!, "An unexpected error occurred: {message}", error.Value.Message)
            );
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred: {errorMessage}", ex.Message);
            returnCode = ReturnCode.Error;
        }

        return (int)returnCode;
    }

    private static IServiceProvider BuildServiceProvider()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddCliServices();
        serviceCollection.AddInfrastructureServices();

        return serviceCollection.BuildServiceProvider();
    }

    private static IProgress<float> GetProgress(ILogger<Program> logger)
    {
        var progress = new Progress<float>(value =>
        {
            logger.LogInformation("Progress: {value:P2}", value);
        });

        return progress;
    }
}