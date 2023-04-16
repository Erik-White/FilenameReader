using FilenameReader.Core;
using FluentValidation;

namespace FilenameReader.Infrastructure.Validators.Test;

[TestFixture]
public class FilePathValidatorTests
{
    [TestCase("")]
    [TestCase(@"C:\Test\")] // No file name or extension
    [TestCase(@"C:\Test\.extension")] // No file name, just extension
    [Description("Invalid paths or filenames should be correctly validated.")]
    public void ValidateAndThrow_ShouldThrow_ValidationException(string filePath)
    {
        var validator = new FilePathValidator();

        Action act = () => validator.ValidateAndThrow(new FilePath(filePath));

        act.Should().ThrowExactly<ValidationException>();
    }

    [TestCase(@"C:\Test")] // File may not have an extension
    [TestCase(@"C:\Test\..extension")] // File name could just be a '.'
    [TestCase(@"C:\Test\test.extension.extension")] // Multiple extensions
    [TestCase(@"C:\Test.test\test.extension")]
    [TestCase(@"C:\Test\\\\test\test.extension")]
    [TestCase(@"C:\Test/test\test.extension")]
    [Description("Unusual, but valid, file paths should be allowed.")]
    public void ValidateAndThrow_ShouldNotThrow_ValidationException(string filePath)
    {
        var validator = new FilePathValidator();

        Action act = () => validator.ValidateAndThrow(new FilePath(filePath));

        act.Should().NotThrow<ValidationException>();
    }
}