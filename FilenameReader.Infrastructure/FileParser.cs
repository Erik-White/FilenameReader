using System.IO.Abstractions;
using System.Text.RegularExpressions;
using FilenameReader.Core;
using FluentValidation;

namespace FilenameReader.Infrastructure;

public class FileParser : IFileParser
{
    private readonly IValidator<FilePath> _filePathValidator;
    private readonly IFileSystem _fileSystem;

    public FileParser(IValidator<FilePath> filePathValidator, IFileSystem fileSystem)
    {
        _filePathValidator = filePathValidator;
        _fileSystem = fileSystem;
    }

    public int CountFileContents(FilePath filePath)
    {
        var validationResult = _filePathValidator.Validate(filePath);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        using var f = _fileSystem.File.OpenRead(filePath.FullPath);

        return ReadLines(f)
            .Where(line => !string.IsNullOrEmpty(line))
            .Sum(line => Regex.Matches(line!, Regex.Escape($"{filePath.Filename}")).Count);
    }

    internal static IEnumerable<string?> ReadLines(FileSystemStream fileStream)
    {
        using var file = new StreamReader(fileStream);

        do
        {
            yield return file.ReadLine();
        } while (!file.EndOfStream);
    }
}