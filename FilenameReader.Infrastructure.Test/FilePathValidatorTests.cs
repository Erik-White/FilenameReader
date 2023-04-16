using FilenameReader.Core;
using FluentValidation.TestHelper;

namespace FilenameReader.Infrastructure.Validators.Test;

[TestFixture]
public class FilePathValidatorTests
{
    [TestCase(null)]
    [TestCase("")]
    [Description("Invalid file paths should be correctly validated.")]
    public void FilePath_ShouldReturn_ValidationErrors(string filePath)
    {
        var sut = new FilePathValidator();

        var validationResult = sut.TestValidate(new FilePath(filePath));

        validationResult.ShouldHaveValidationErrorFor(p => p.FullPath);
    }

    [TestCase(@"\")] // No file name or extension
    [TestCase(@"C:\Test\")] // No file name or extension
    [TestCase(@"C:\Test\.extension")] // No file name, just extension
    [Description("Invalid filenames should be correctly validated.")]
    public void Filename_ShouldReturn_ValidationErrors(string filePath)
    {
        var sut = new FilePathValidator();

        var validationResult = sut.TestValidate(new FilePath(filePath));

        validationResult.ShouldHaveValidationErrorFor(p => p.Filename);
    }

    [TestCase(@"C:\Test")] // File may not have an extension
    [TestCase(@"C:\Test\..extension")] // File name could just be a '.'
    [TestCase(@"C:\Test\test.extension.extension")] // Multiple extensions
    [TestCase(@"C:\Test.test\test.extension")]
    [TestCase(@"C:\Test\\\\test\test.extension")]
    [TestCase(@"!#$%&'+,-.;=@[]^_`{}~€‚ƒ„…†‡ˆ‰Š‹ŒŽ‘’“”•–—˜™š›œžŸ¡¢£¤¥¦§¨©ª«¬­®¯°±²³´µ¶·¸¹º»¼½¾¿ÀÆÇ×ØÞßàæçð÷øþ💚🇮🇳‱𐠜‽ⴃ𐬼𠁨𐒾")] // Unicode characters
    // Windows paths were restrictured to <= 255 characters, should not be a problem in .Net Core
    [TestCase("9CBZLh9ZuqFmza9L9drqx0dE55FMst93zHp4BJAHPC6aDKkEaA7qMKML046eYRrLKkrcEYdhPlNbB8eOYkxVltjdgWb3TMrtf2oK5H8xe2vWFcSudX6MG7yx8wWz4m6PzB" +
        "jPwjCmHh0YAAPg8vu8YKb4yaDr3K5VRvdW4NzsoRBvG6EPmRijKqrKCcPqfXqOZTUcptcZk6E3zpHMfHyUvvFpfSk7c04fYPxYSSlHPwQ2QZHRY0ySt3BhdBZmgwTmcsWk")]
    [Description("Unusual, but valid, file paths should be allowed.")]
    public void Validate_ShouldNotReturn_ValidationErrors(string filePath)
    {
        var sut = new FilePathValidator();

        var validationResult = sut.TestValidate(new FilePath(filePath));

        validationResult.ShouldNotHaveAnyValidationErrors();
    }
}