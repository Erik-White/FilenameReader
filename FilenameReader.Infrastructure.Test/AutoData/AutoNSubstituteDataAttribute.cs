using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.NUnit3;

namespace FilenameReader.Infrastructure.Test.AutoData;

public class AutoNSubstituteDataAttribute : AutoDataAttribute
{
    public AutoNSubstituteDataAttribute(Action<IFixture> fixtureAction) : base(() =>
    {
        var fixture = new Fixture();

        fixture.Customize(new AutoNSubstituteCustomization
        {
            ConfigureMembers = true,
            GenerateDelegates = true
        });

        fixtureAction(fixture);

        return fixture;
    })
    {
    }
}
