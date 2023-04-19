using System.IO.Abstractions;
using FilenameReader.Core;
using FilenameReader.Infrastructure.Validators;
using FluentValidation;
using OneOf.Types;
using OneOf;

namespace FilenameReader.Infrastructure;

public class FileTextSearcher : IFileTextSearcher
{
    private readonly IValidator<FilePath> _filePathValidator;
    private readonly ITextSearcher _textSearcher;
    private readonly IFileSystem _fileSystem;

    public FileTextSearcher(IValidator<FilePath> filePathValidator, ITextSearcher textSearcher, IFileSystem fileSystem)
    {
        _filePathValidator = filePathValidator;
        _textSearcher = textSearcher;
        _fileSystem = fileSystem;
    }

    public OneOf<int, ValidationFailed, NotFound, Error<Exception>> CountFileContents(
        FilePath filePath,
        string searchValue,
        TextSearchOptions searchOptions = default,
        IProgress<float>? progress = null)
    {
        var validationResult = _filePathValidator.Validate(filePath);
        if (!validationResult.IsValid)
        {
            return new ValidationFailed(validationResult.Errors);
        }

        try
        {
            using var fileStream = _fileSystem.File.OpenRead(filePath.FullPath);

            return _textSearcher.CountStreamContents(fileStream, searchValue, searchOptions, progress);
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
        IProgress<float>? progress = null,
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

            return await _textSearcher.CountStreamContentsAsync(fileStream, searchValue, searchOptions, progress, cancellationToken);
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
}