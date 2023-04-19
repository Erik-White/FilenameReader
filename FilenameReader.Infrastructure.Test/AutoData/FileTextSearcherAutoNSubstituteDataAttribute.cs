using AutoFixture;
using FilenameReader.Infrastructure.Test.Customizations;

namespace FilenameReader.Infrastructure.Test.AutoData;

public class FileTextSearcherAutoNSubstituteDataAttribute : AutoNSubstituteDataAttribute
{
    public FileTextSearcherAutoNSubstituteDataAttribute() : base(fixture =>
    {
        fixture.Customize(new MockFileSystemCustomization());
        fixture.Customize(new FilePathValidatorOverrideCustomization());
        fixture.Register<ITextSearcher>(() => fixture.Create<RegexTextSearcher>());
    })
    {
    }
}