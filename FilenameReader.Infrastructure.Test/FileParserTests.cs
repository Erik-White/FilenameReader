namespace FilenameReader.Infrastructure.Test;

[TestFixture]
public class FileParserTests
{
    [Test]
    public void EmptyFilePath_Should_ThrowArgumentOutOfRangeException()
    {
        var parser = new FileParser();

        Action act = () => parser.CountFileContents(string.Empty);

        act.Should().ThrowExactly<ArgumentOutOfRangeException>();
    }

    [Test]
    public void MissingFile_Should_ThrowArgumentOutOfRangeException()
    {
        var parser = new FileParser();

        Action act = () => parser.CountFileContents(Guid.NewGuid().ToString());

        act.Should().ThrowExactly<FileNotFoundException>();
    }
}