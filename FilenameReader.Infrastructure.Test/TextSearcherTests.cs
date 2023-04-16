using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Text;
using AutoFixture.NUnit3;
using FilenameReader.Core;
using FilenameReader.Infrastructure.Test.AutoData;
using FilenameReader.Infrastructure.Validators;
using FluentAssertions.OneOf;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using OneOf.Types;

namespace FilenameReader.Infrastructure.Test;

[TestFixture]
public class TextSearcherTests
{
    private const string LoremIpsum = "Lorem ipsum dolor sit amet, consectetur loremlorem adipiscing elit. Nullam loremus lacinia venenatis nulla eu lorem hendrerit.";

    [Test, TextSearcherAutoNSubstituteData]
    public void ValidatorError_ShouldReturn_ValidationFailed([Frozen] IValidator<FilePath> validator, TextSearcher sut, FilePath filePath)
    {
        var expectedInvalidResult = new ValidationFailure(nameof(FilePath.FullPath), "Failed");
        var invalidResult = new ValidationResult(new[] { expectedInvalidResult });
        validator.Validate(Arg.Any<FilePath>()).Returns(invalidResult);

        var result = sut.CountFileContents(filePath, filePath.Filename);

        result
            .Should().Be<ValidationFailed>()
            .And.Errors.First()
                .Should().Be(expectedInvalidResult);
    }

    [Test, TextSearcherAutoNSubstituteData]
    public void MissingFile_ShouldReturn_NotFound(TextSearcher sut, FilePath filePath)
    {
        var result = sut.CountFileContents(filePath, filePath.Filename);

        result.Should().Be<NotFound>();
    }

    [Test, TextSearcherAutoNSubstituteData]
    public void NoFileShare_ShouldReturn_Error([Frozen] IFileSystem fileSystem, TextSearcher sut, FilePath filePath)
    {
        (fileSystem as MockFileSystem)?.AddFile(filePath.FullPath, new MockFileData(string.Empty) { AllowedFileShare = FileShare.None });

        var result = sut.CountFileContents(filePath, filePath.Filename);

        result
            .Should().Be<Error<Exception>>()
            .And.Value
                .Should().BeOfType<IOException>();
    }

    [Test, TextSearcherAutoNSubstituteData]
    public void EmptyFile_ShouldReturn_Zero([Frozen] IFileSystem fileSystem, TextSearcher sut, FilePath filePath)
    {
        (fileSystem as MockFileSystem)?.AddEmptyFile(filePath.FullPath);

        var result = sut.CountFileContents(filePath, filePath.Filename);

        result.Value.Should().Be(0);
    }

    [Test, TextSearcherAutoNSubstituteData]
    public void SubStrings_ShouldReturn_ExpectedCount([Frozen] IFileSystem fileSystem, TextSearcher sut)
    {
        const string filename = "aa";
        const int expectedCount = 1;
        var searchOptions = new TextSearchOptions(CaseInsensitive: false, RespectWordBoundaries: true);

        (fileSystem as MockFileSystem)?.AddFile(filename, new MockFileData("aa aaa aaaa"));

        var result = sut.CountFileContents(new FilePath(filename), filename, searchOptions);

        result.Value.Should().Be(expectedCount, "Word boundaries should be respected.");
    }

    [Test, TextSearcherAutoNSubstituteData]
    public void TextSearchOptions_ShouldReturn_ExpectedCount([Frozen] IFileSystem fileSystem, TextSearcher sut)
    {
        const string filename = "lorem";
        const int expectedCount = 2;
        var searchOptions = new TextSearchOptions(CaseInsensitive: true, RespectWordBoundaries: true);

        var fileContents = new MockFileData(LoremIpsum);
        (fileSystem as MockFileSystem)?.AddFile(filename, fileContents);

        var result = sut.CountFileContents(new FilePath(filename), filename, searchOptions);

        result.Value.Should().Be(expectedCount, "All instances of the filename should be counted, regardless of casing.");
    }

    [Test, TextSearcherAutoNSubstituteData]
    public void FileContentsWithEncoding_ShouldReturn_ExpectedCount([Frozen] IFileSystem fileSystem, TextSearcher sut)
    {
        const string filename = "lorem";
        const int expectedCount = 4;
        
        // It would be nice to generate test cases for each encoding instead.
        // Easy to do with [Values] in NUnit, but difficult to combine with AutoFixture
        foreach (var encodingInfo in Encoding.GetEncodings())
        {
            var fileContents = new MockFileData(LoremIpsum, encodingInfo.GetEncoding());
            (fileSystem as MockFileSystem)?.AddFile(filename, fileContents);

            var result = sut.CountFileContents(new FilePath(filename), filename);

            result.Value.Should().Be(
                expectedCount,
                "The expected number of name instances should be found in files with the text encoding {0}",
                encodingInfo.Name);
        }
    }
}