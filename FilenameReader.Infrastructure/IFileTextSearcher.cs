using FilenameReader.Core;
using FilenameReader.Infrastructure.Validators;
using OneOf;
using OneOf.Types;

namespace FilenameReader.Infrastructure;

public interface IFileTextSearcher
{
    OneOf<int, ValidationFailed, NotFound, Error<Exception>> CountFileContents(
        FilePath filePath,
        string searchValue,
        TextSearchOptions searchOptions = default,
        IProgress<float>? progress = null);

    Task<OneOf<int, ValidationFailed, NotFound, Error<Exception>>> CountFileContentsAsync(
        FilePath filePath,
        string searchValue,
        TextSearchOptions searchOptions = default,
        IProgress<float>? progress = null,
        CancellationToken cancellationToken = default);
}