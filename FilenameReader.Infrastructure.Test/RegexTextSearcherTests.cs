using System.IO.Abstractions.TestingHelpers;
using System.Text;
using FilenameReader.Core;
using FilenameReader.Infrastructure.Test.AutoData;
using FluentAssertions.OneOf;

namespace FilenameReader.Infrastructure.Test;

[TestFixture]
public class RegexTextSearcherTests
{
    private const string LoremIpsum = "Lorem ipsum dolor sit amet, consectetur loremlorem adipiscing elit.\r Nullam loremus lacinia venenatis nulla eu lorem hendrerit.";

    [Test, TextSearcherAutoNSubstituteData]
    public void SubStrings_ShouldReturn_ExpectedCount(MockFileSystem fileSystem, RegexTextSearcher sut)
    {
        const string searchText = "aa";
        const int expectedCount = 1;
        var searchOptions = new TextSearchOptions(CaseInsensitive: false, RespectWordBoundaries: true);

        fileSystem.AddFile(searchText, new MockFileData("aa aaa aaaa"));
        using var stream = fileSystem.File.OpenRead(searchText);

        var result = sut.CountStreamContents(stream, searchText, searchOptions);

        result.Should().Be(expectedCount, "Word boundaries should be respected.");
    }

    [Test, TextSearcherAutoNSubstituteData]
    public void TextSearchOptions_ShouldReturn_ExpectedCount(MockFileSystem fileSystem, RegexTextSearcher sut)
    {
        const string searchText = "lorem";
        const int expectedCount = 2;
        var searchOptions = new TextSearchOptions(CaseInsensitive: true, RespectWordBoundaries: true);

        var fileContents = new MockFileData(LoremIpsum);
        fileSystem.AddFile(searchText, fileContents);
        using var stream = fileSystem.File.OpenRead(searchText);

        var result = sut.CountStreamContents(stream, searchText, searchOptions);

        result.Should().Be(expectedCount, "All instances of the filename should be counted, regardless of casing.");
    }

    [Test, TextSearcherAutoNSubstituteData]
    public void FileContentsWithEncoding_ShouldReturn_ExpectedCount(MockFileSystem fileSystem, RegexTextSearcher sut)
    {
        const string searchText = "lorem";
        const int expectedCount = 4;

        // It would be nice to generate test cases for each encoding instead.
        // Easy to do with [Values] in NUnit, but difficult to combine with AutoFixture
        foreach (var encodingInfo in Encoding.GetEncodings())
        {
            var fileContents = new MockFileData(LoremIpsum, encodingInfo.GetEncoding());
            fileSystem.AddFile(searchText, fileContents);
            using var stream = fileSystem.File.OpenRead(searchText);

            var result = sut.CountStreamContents(stream, searchText, new TextSearchOptions());

            result.Should().Be(
                expectedCount,
                "The expected number of name instances should be found in files with the text encoding {0}",
                encodingInfo.Name);

            fileSystem.File.Delete(searchText);
        }
    }

    [Test, TextSearcherAutoNSubstituteData]
    public async Task SyncAndAsync_ShouldReturn_IdenticalCount(MockFileSystem fileSystem, RegexTextSearcher sut)
    {
        const string searchText = "lorem";
        var searchOptions = new TextSearchOptions(CaseInsensitive: false, RespectWordBoundaries: false);

        var fileContents = new MockFileData(LoremIpsum);
        fileSystem.AddFile(searchText, fileContents);
        using var stream = fileSystem.File.OpenRead(searchText);

        var syncResult = sut.CountStreamContents(stream, searchText, searchOptions);
        stream.Seek(0, SeekOrigin.Begin);
        var asyncResult = await sut.CountStreamContentsAsync(stream, searchText, searchOptions);

        syncResult.Should().Be(asyncResult, "Both synchronous and asyncronous methods should return the same results.");
    }
}