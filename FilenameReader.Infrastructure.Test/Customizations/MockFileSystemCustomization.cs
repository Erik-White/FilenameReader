using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using AutoFixture;

namespace FilenameReader.Infrastructure.Test.Customizations;

public class MockFileSystemCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Register<IFileSystem>(() => new MockFileSystem());
    }
}
