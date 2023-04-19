using FilenameReader.Core;

namespace FilenameReader.Infrastructure
{
    public interface ITextSearcher
    {
        int CountStreamContents(Stream stream, string searchValue, TextSearchOptions searchOptions);

        Task<int> CountStreamContentsAsync(
            Stream stream,
            string searchValue,
            TextSearchOptions searchOptions,
            CancellationToken cancellationToken);
    }
}