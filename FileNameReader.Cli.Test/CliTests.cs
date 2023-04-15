using FilenameReaderCli;

namespace FilenameReader.Cli.Test;

[TestFixture]
public class CliTests
{
    [TestCase("empty_file.txt", 0)]
    [TestCase("lorem.txt", 7)]
    [TestCase("jfif.jpeg", 0)]
    public void Main_Should_NotThrow_AndOutput_ExpectedCount(string filename, int expectedCount)
    {
        var filePath = Path.Join(TestHelper.TestDataPath, filename);
        using var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        Action act = () => Program.Main(new [] { filePath });

        act.Should().NotThrow();
        ParseStringWriterCount(stringWriter).Should().Be(expectedCount);
    }

    private static int ParseStringWriterCount(StringWriter stringWriter)
        => int.TryParse($"{stringWriter}{Environment.NewLine}", out var count)
            ? count
            : 0;
}