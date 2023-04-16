using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
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
public class FileParserTests
{
    private const string LoremIpsum = "Lorem ipsum dolor sit amet, consectetur loremlorem adipiscing elit. Nullam loremus lacinia venenatis nulla eu lorem hendrerit.";

    [Test, FileParserAutoNSubstituteData]
    public void ValidatorError_ShouldReturn_ValidationFailed([Frozen] IValidator<FilePath> validator, FileParser sut, string filePath)
    {
        var expectedInvalidResult = new ValidationFailure(nameof(FilePath.FullPath), "Failed");
        var invalidResult = new ValidationResult(new[] { expectedInvalidResult });
        validator.Validate(Arg.Any<FilePath>()).Returns(invalidResult);

        var result = sut.CountFileContents(new FilePath(filePath));

        result
            .Should().Be<ValidationFailed>()
            .And.Errors.First()
                .Should().Be(expectedInvalidResult);
    }

    [Test, FileParserAutoNSubstituteData]
    public void MissingFile_ShouldReturn_NotFound(FileParser sut, string filePath)
    {
        var result = sut.CountFileContents(new FilePath(filePath));

        result.Should().Be<NotFound>();
    }

    [Test, FileParserAutoNSubstituteData]
    public void IOException_ShouldReturn_Error([Frozen] IFileSystem fileSystem, FileParser sut, string filePath)
    {
        (fileSystem as MockFileSystem)?.AddFile(filePath, new MockFileData(string.Empty) { AllowedFileShare = FileShare.None });

        var result = sut.CountFileContents(new FilePath(filePath));

        result
            .Should().Be<Error<Exception>>()
            .And.Value
                .Should().BeOfType<IOException>();
    }

    [Test, FileParserAutoNSubstituteData]
    public void EmptyFile_ShouldReturn_Zero([Frozen] IFileSystem fileSystem, FileParser sut, string filePath)
    {
        (fileSystem as MockFileSystem)?.AddEmptyFile(filePath);

        var result = sut.CountFileContents(new FilePath(filePath));

        result.Value.Should().Be(0);
    }

    [Test, FileParserAutoNSubstituteData]
    public void SubStrings_ShouldReturn_ExpectedCount([Frozen] IFileSystem fileSystem, FileParser sut)
    {
        const string filename = "aa";
        const int expectedCount = 1;

        (fileSystem as MockFileSystem)?.AddFile(filename, new MockFileData("aa aaa aaaa"));

        var result = sut.CountFileContents(new FilePath(filename));

        result.Value.Should().Be(expectedCount, "Word boundaries should be respected.");
    }

    [Test, FileParserAutoNSubstituteData]
    public void IgnoreCase_ShouldReturn_ExpectedCount([Frozen] IFileSystem fileSystem, FileParser sut)
    {
        const string filename = "lorem";
        const int expectedCount = 2;

        var fileContents = new MockFileData(LoremIpsum);
        (fileSystem as MockFileSystem)?.AddFile(filename, fileContents);

        var result = sut.CountFileContents(new FilePath(filename), ignoreCase: true);

        result.Value.Should().Be(expectedCount, "All instances of the filename should be counted, regardless of casing.");
    }

    [Test, FileParserAutoNSubstituteData]
    public void FileContentsWithEncoding_ShouldReturn_ExpectedCount([Frozen] IFileSystem fileSystem, FileParser sut)
    {
        const string filename = "lorem";
        const int expectedCount = 1;
        
        // It would be nice to generate test cases for each encoding instead.
        // Easy to do with [Values] in NUnit, but difficult to combine with AutoFixture
        foreach (var encodingInfo in System.Text.Encoding.GetEncodings())
        {
            var fileContents = new MockFileData(LoremIpsum, encodingInfo.GetEncoding());
            (fileSystem as MockFileSystem)?.AddFile(filename, fileContents);

            var result = sut.CountFileContents(new FilePath(filename));

            result.Value.Should().Be(
                expectedCount,
                "The expected number of name instances should be found in files with the text encoding {0}",
                encodingInfo.Name);
        }
    }
}