using AutoFixture;
using FilenameReader.Core;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;

namespace FilenameReader.Infrastructure.Test.Customizations;

/// <summary>
/// Always returns valid for any value and does not throw
/// </summary>
public class FilePathValidatorOverrideCustomization : ICustomization
{
    public static ValidationResult ValidResult => new();

    public void Customize(IFixture fixture)
    {
        var validator = fixture.Create<IValidator<FilePath>>();
        validator.Validate(Arg.Any<FilePath>()).Returns(ValidResult);
        validator.ValidateAsync(Arg.Any<FilePath>()).Returns(ValidResult);

        fixture.Register(() => validator);
    }
}
