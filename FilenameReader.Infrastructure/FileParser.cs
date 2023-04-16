using System.IO.Abstractions;
using System.Text.RegularExpressions;
using FilenameReader.Core;
using FilenameReader.Infrastructure.Validators;
using FluentValidation;
using OneOf;
using OneOf.Types;

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

    public OneOf<int, ValidationFailed, NotFound, Error<Exception>> CountFileContents(FilePath filePath, bool ignoreCase = false)
    {
        var validationResult = _filePathValidator.Validate(filePath);
        if (!validationResult.IsValid)
        {
            return new ValidationFailed(validationResult.Errors);
        }

        try
        {
            using var fileStream = _fileSystem.File.OpenRead(filePath.FullPath);

            return CountFileContents(fileStream, filePath, ignoreCase);
        }
        catch (FileNotFoundException)
        {
            return new NotFound();
        }
        catch (Exception ex)
        {
            // Could be lots of different errors related to file access.
            // Would be nice to validate before attempting to read the file,
            // but difficult to do both as an atomic operation. 
            return new Error<Exception>(ex);
        }
    }

    internal static int CountFileContents(Stream fileStream, FilePath filePath, bool ignoreCase)
    {
        const string FilenameRegexPattern = @"\b{0}\b";

        var regexCaseOption = ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None;
        var filenameRegex = string.Format(FilenameRegexPattern, Regex.Escape(filePath.Filename));

        return ReadLines(fileStream)
            .Where(line => !string.IsNullOrEmpty(line))
            .Sum(line => Regex.Matches(line!, filenameRegex, regexCaseOption).Count);
    }

    internal static IEnumerable<string?> ReadLines(Stream fileStream)
    {
        using var file = new StreamReader(fileStream);

        while (!file.EndOfStream)
        {
            yield return file.ReadLine();
        }
    }
}