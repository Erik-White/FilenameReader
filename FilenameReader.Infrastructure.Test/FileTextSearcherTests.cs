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
public class FileTextSearcherTests
{
    [Test, FileTextSearcherAutoNSubstituteData]
    public void ValidatorError_ShouldReturn_ValidationFailed([Frozen] IValidator<FilePath> validator, FileTextSearcher sut, FilePath filePath)
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

    [Test, FileTextSearcherAutoNSubstituteData]
    public void MissingFile_ShouldReturn_NotFound(FileTextSearcher sut, FilePath filePath)
    {
        var result = sut.CountFileContents(filePath, filePath.Filename);

        result.Should().Be<NotFound>();
    }

    [Test, FileTextSearcherAutoNSubstituteData]
    public void NoFileShare_ShouldReturn_Error([Frozen] IFileSystem fileSystem, FileTextSearcher sut, FilePath filePath)
    {
        (fileSystem as MockFileSystem)?.AddFile(filePath.FullPath, new MockFileData(string.Empty) { AllowedFileShare = FileShare.None });

        var result = sut.CountFileContents(filePath, filePath.Filename);

        result
            .Should().Be<Error<Exception>>()
            .And.Value
                .Should().BeOfType<IOException>();
    }

    [Test, FileTextSearcherAutoNSubstituteData]
    public void EmptyFile_ShouldReturn_Zero([Frozen] IFileSystem fileSystem, FileTextSearcher sut, FilePath filePath)
    {
        (fileSystem as MockFileSystem)?.AddEmptyFile(filePath.FullPath);

        var result = sut.CountFileContents(filePath, filePath.Filename);

        result.Value.Should().Be(0);
    }
}