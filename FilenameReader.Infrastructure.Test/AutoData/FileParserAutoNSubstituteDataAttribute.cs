using FilenameReader.Infrastructure.Test.Customizations;

namespace FilenameReader.Infrastructure.Test.AutoData;

public class FileParserAutoNSubstituteDataAttribute : AutoNSubstituteDataAttribute
{
    public FileParserAutoNSubstituteDataAttribute() : base(fixture =>
    {
        fixture.Customize(new MockFileSystemCustomization());
        fixture.Customize(new FilePathValidatorOverrideCustomization());
    })
    {
    }
}