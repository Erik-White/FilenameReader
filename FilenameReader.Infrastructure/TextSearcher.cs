using System.IO.Abstractions;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using FilenameReader.Core;
using FilenameReader.Infrastructure.Validators;
using FluentValidation;
using OneOf;
using OneOf.Types;

namespace FilenameReader.Infrastructure;

public class TextSearcher : ITextSearcher
{
    private readonly IValidator<FilePath> _filePathValidator;
    private readonly IFileSystem _fileSystem;

    public TextSearcher(IValidator<FilePath> filePathValidator, IFileSystem fileSystem)
    {
        _filePathValidator = filePathValidator;
        _fileSystem = fileSystem;
    }

    public OneOf<int, ValidationFailed, NotFound, Error<Exception>> CountFileContents(FilePath filePath, string searchValue, TextSearchOptions searchOptions = default)
    {
        var validationResult = _filePathValidator.Validate(filePath);
        if (!validationResult.IsValid)
        {
            return new ValidationFailed(validationResult.Errors);
        }

        try
        {
            using var fileStream = _fileSystem.File.OpenRead(filePath.FullPath);

            return CountStreamContents(fileStream, searchValue, searchOptions);
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

    public async Task<OneOf<int, ValidationFailed, NotFound, Error<Exception>>> CountFileContentsAsync(
        FilePath filePath,
        string searchValue,
        TextSearchOptions searchOptions = default,
        CancellationToken cancellationToken = default)
    {
        var validationResult = _filePathValidator.Validate(filePath);
        if (!validationResult.IsValid)
        {
            return new ValidationFailed(validationResult.Errors);
        }

        try
        {
            using var fileStream = _fileSystem.File.OpenRead(filePath.FullPath);

            return await CountStreamContentsAsync(fileStream, searchValue, searchOptions, cancellationToken);
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

    /// <summary>
    /// Count the instances of <paramref name="searchValue"/> in the stream data.
    /// Assumes that data has a valid text encoding.
    /// </summary>
    public int CountStreamContents(Stream stream, string searchValue, TextSearchOptions searchOptions)
    {
        var regexCaseOption = searchOptions.CaseInsensitive ? RegexOptions.IgnoreCase : RegexOptions.None;
        var filenameRegexPattern = GetSearchTextRegexPattern(searchValue, searchOptions);

        return ReadLines(stream)
            .Where(line => !string.IsNullOrEmpty(line))
            .Sum(line => Regex.Matches(line!, filenameRegexPattern, regexCaseOption).Count);
    }

    /// <summary>
    /// Count the instances of <paramref name="searchValue"/> in the stream data.
    /// Assumes that data has a valid text encoding.
    /// </summary>
    public ValueTask<int> CountStreamContentsAsync(Stream stream, string searchValue, TextSearchOptions searchOptions, CancellationToken cancellationToken)
    {
        var regexCaseOption = searchOptions.CaseInsensitive ? RegexOptions.IgnoreCase : RegexOptions.None;
        var filenameRegexPattern = GetSearchTextRegexPattern(searchValue, searchOptions);

        return ReadLinesAsync(stream, cancellationToken)
            .Where(line => !string.IsNullOrEmpty(line))
            .SumAsync(line => Regex.Matches(line!, filenameRegexPattern, regexCaseOption).Count, cancellationToken);
    }

    internal static IEnumerable<string?> ReadLines(Stream stream)
    {
        using var file = new StreamReader(stream);

        while (!file.EndOfStream)
        {
            yield return file.ReadLine();
        }
    }

    internal static async IAsyncEnumerable<string?> ReadLinesAsync(Stream stream, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using var file = new StreamReader(stream);

        while (!file.EndOfStream)
        {
            yield return await file.ReadLineAsync(cancellationToken);
        }
    }

    private static string GetSearchTextRegexPattern(string filename, TextSearchOptions searchOptions)
    {
        var regexBoundaryPattern = searchOptions.RespectWordBoundaries
            ? @"\b{0}\b"
            : @"{0}";

        return string.Format(regexBoundaryPattern, Regex.Escape(filename));
    }
}