using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using FilenameReader.Core;

namespace FilenameReader.Infrastructure;

public class RegexTextSearcher : ITextSearcher
{
    int TextBufferSize { get; set; } = 1024 * 32;

    /// <summary>
    /// Count the instances of <paramref name="searchValue"/> in the stream data.
    /// Assumes that data has a valid text encoding.
    /// </summary>
    public int CountStreamContents(Stream stream, string searchValue, TextSearchOptions searchOptions)
    {
        var regexCaseOption = searchOptions.CaseInsensitive ? RegexOptions.IgnoreCase : RegexOptions.None;
        var filenameRegexPattern = GetSearchTextRegexPattern(searchValue, searchOptions);

        return ReadLines(stream, searchValue.Length)
            .Where(line => !string.IsNullOrEmpty(line))
            .Sum(line => Regex.Matches(line!, filenameRegexPattern, regexCaseOption).Count);
    }

    /// <summary>
    /// Count the instances of <paramref name="searchValue"/> in the stream data.
    /// Assumes that data has a valid text encoding.
    /// </summary>
    public async Task<int> CountStreamContentsAsync(Stream stream, string searchValue, TextSearchOptions searchOptions, CancellationToken cancellationToken)
    {
        var regexCaseOption = searchOptions.CaseInsensitive ? RegexOptions.IgnoreCase : RegexOptions.None;
        var filenameRegexPattern = GetSearchTextRegexPattern(searchValue, searchOptions);
        var count = 0;

        using var queue = new BlockingCollection<string>(boundedCapacity: 10000);

        // Add text to the queue as fast as possible
        var readLines = Task.Run(async () =>
        {
            try
            {
                await ReadLinesAsync(stream, searchValue.Length, cancellationToken)
                    .Where(line => !string.IsNullOrEmpty(line))
                    .ForEachAsync(line => queue.Add(line!), cancellationToken);
            }
            finally
            {
                queue.CompleteAdding();
            }
        }, cancellationToken);

        // Process the queue as fast as possible, in parallel
        queue
            .GetConsumingEnumerable(cancellationToken) // From ParallelExtensions, reduces syncronisation overhead
            .AsParallel()
            .WithDegreeOfParallelism(Environment.ProcessorCount - 2)
            .ForAll(line => Interlocked.Add(ref count, Regex.Matches(line!, filenameRegexPattern, regexCaseOption).Count));

        await readLines;

        return count;
    }

    internal IEnumerable<string?> ReadLines(Stream stream, int boundarySize)
    {
        char[] buffer = new char[TextBufferSize];
        char[] boundaryBuffer = new char[boundarySize];

        using var reader = new StreamReader(stream, leaveOpen: true);

        while (reader.ReadBlock(buffer, 0, buffer.Length) > 0)
        {
            yield return string.Concat(new string(boundaryBuffer), new string(buffer));
            // Store the last section of text to avoid word boundary issues
            boundaryBuffer = buffer[new Range(buffer.Length - boundarySize, buffer.Length)];
        }
    }

    internal async IAsyncEnumerable<string?> ReadLinesAsync(
        Stream stream,
        int boundarySize,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        char[] buffer = new char[TextBufferSize];
        char[] boundaryBuffer = new char[boundarySize];

        using var reader = new StreamReader(stream, leaveOpen: true);

        while ((await reader.ReadBlockAsync(buffer, cancellationToken)) > 0)
        {
            yield return string.Concat(new string(boundaryBuffer), new string(buffer));
            // Store the last section of text to avoid word boundary issues
            boundaryBuffer = buffer[new Range(buffer.Length - boundarySize, buffer.Length)];
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