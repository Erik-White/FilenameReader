using FilenameReader.Infrastructure.Test.Customizations;

namespace FilenameReader.Infrastructure.Test.AutoData;

public class TextSearcherAutoNSubstituteDataAttribute : AutoNSubstituteDataAttribute
{
    public TextSearcherAutoNSubstituteDataAttribute() : base(fixture =>
    {
        fixture.Customize(new MockFileSystemCustomization());
        fixture.Customize(new FilePathValidatorOverrideCustomization());
    })
    {
    }
}