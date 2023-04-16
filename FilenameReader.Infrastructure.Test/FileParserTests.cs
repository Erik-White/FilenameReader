using AutoFixture.NUnit3;
using FilenameReader.Core;
using FilenameReader.Infrastructure.Test.AutoData;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;

namespace FilenameReader.Infrastructure.Test;

[TestFixture]
public class FileParserTests
{
    [Test, FileParserAutoNSubstituteData]
    public void ValidatorError_ShouldThrow_ValidationException([Frozen] IValidator<FilePath> validator, FileParser sut, string filePath)
    {
        var expectedInvalidProperty = nameof(FilePath.FullPath);
        var invalidResult = new ValidationResult(new[] { new ValidationFailure(expectedInvalidProperty, "Failed") });
        validator.Validate(Arg.Any<FilePath>()).Returns(invalidResult);

        Action act = () => sut.CountFileContents(new FilePath(filePath));

        act
            .Should()
            .ThrowExactly<ValidationException>()
            .Which.Errors.First().PropertyName
                .Should()
                .Be(expectedInvalidProperty);
    }

    [Test, FileParserAutoNSubstituteData]
    public void MissingFile_ShouldThrow_FileNotFoundException(FileParser sut, string filePath)
    {
        Action act = () => sut.CountFileContents(new FilePath(filePath));

        act.Should().ThrowExactly<FileNotFoundException>();
    }
}