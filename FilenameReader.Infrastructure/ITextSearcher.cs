using FilenameReader.Core;

namespace FilenameReader.Infrastructure
{
    public interface ITextSearcher
    {
        int CountStreamContents(
            Stream stream,
            string searchValue,
            TextSearchOptions searchOptions,
            IProgress<float>? progress = null);

        Task<int> CountStreamContentsAsync(
            Stream stream,
            string searchValue,
            TextSearchOptions searchOptions,
            IProgress<float>? progress = null,
            CancellationToken cancellationToken = default);
    }
}