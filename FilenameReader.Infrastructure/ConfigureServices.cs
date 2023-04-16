using System.IO.Abstractions;
using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace FilenameReader.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IFileSystem, FileSystem>();
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddScoped<ITextSearcher, TextSearcher>();

        return services;
    }
}