using System.IO.Abstractions.TestingHelpers;
using FilenameReader.Core;
using FilenameReader.Infrastructure.Validators;

namespace FilenameReader.Infrastructure.Test;

[TestFixture]
public class FileParserTests
{
    [Test]
    public void MissingFile_ShouldThrow_FileNotFoundException()
    {
        var parser = new FileParser(new FilePathValidator(), new MockFileSystem());

        Action act = () => parser.CountFileContents(new FilePath(Guid.NewGuid().ToString()));

        act.Should().ThrowExactly<FileNotFoundException>();
    }
}