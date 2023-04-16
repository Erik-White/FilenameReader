using FilenameReaderCli;

namespace FilenameReader.Cli.Test;

[TestFixture]
public class CliTests
{
    [TestCase("", 0)]
    [TestCase("empty_file.txt", 0)]
    [TestCase("jfif.jpg", 0)]
    [TestCase("lorem.txt", 7)]
    public void Main_Should_NotThrow_AndOutput_ExpectedCount(string filename, int expectedCount)
    {
        var filePath = Path.Join(TestHelper.TestDataPath, filename);
        using var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        Action act = () => Program.Main(new[] { filePath });

        act.Should().NotThrow();
        ParseStringWriterCount(stringWriter).Should().Be(expectedCount);
    }

    private static int ParseStringWriterCount(StringWriter stringWriter)
    {
        var line = stringWriter.ToString()
            .Split(Environment.NewLine)
            .FirstOrDefault(line => line.Trim().StartsWith("Filename count"));

        // Try to convert the last character in the line
        return !string.IsNullOrEmpty(line) && int.TryParse($"{line[^1]}{Environment.NewLine}", out var count)
            ? count
            : 0;
    }
}