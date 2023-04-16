using FilenameReader.Core;
using FilenameReader.Infrastructure.Validators;
using OneOf;
using OneOf.Types;

namespace FilenameReader.Infrastructure
{
    public interface ITextSearcher
    {
        OneOf<int, ValidationFailed, NotFound, Error<Exception>> CountFileContents(
            FilePath filePath,
            string searchValue,
            TextSearchOptions searchOptions = default);

        Task<OneOf<int, ValidationFailed, NotFound, Error<Exception>>> CountFileContentsAsync(
            FilePath filePath,
            string searchValue,
            TextSearchOptions searchOptions = default,
            CancellationToken cancellationToken = default);

        int CountStreamContents(Stream stream, string searchValue, TextSearchOptions searchOptions);

        ValueTask<int> CountStreamContentsAsync(
            Stream stream,
            string searchValue,
            TextSearchOptions searchOptions,
            CancellationToken cancellationToken);
    }
}