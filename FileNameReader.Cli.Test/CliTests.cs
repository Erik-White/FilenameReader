using FilenameReaderCli;
using FluentAssertions.Execution;

namespace FilenameReader.Cli.Test;

[TestFixture]
public class CliTests
{
    [TestCase("lorem.txt", 7)]
    [TestCase("", 0)]
    [TestCase("empty_file.txt", 0)]
    [TestCase("jfif.jpg", 0)]
    public async Task Main_Should_ReturnSuccess_AndOutput_ExpectedCount(string filename, int expectedCount)
    {
        var filePath = Path.Join(TestHelper.TestDataPath, filename);
        using var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        var result = await Program.Main(new[] { filePath });

        using (new AssertionScope())
        {
            result.Should().Be((int)Program.ReturnCode.Success);
            // Not a great way to check the result. but sufficient for a quick test
            ParseStringWriterCount(stringWriter).Should().Be(expectedCount);
        }
    }

    private static int ParseStringWriterCount(StringWriter stringWriter)
    {
        var line = stringWriter.ToString()
            .Split(Environment.NewLine)
            .LastOrDefault(line => line.Trim().StartsWith("Filename count"));

        // Try to convert the last character in the line
        return !string.IsNullOrEmpty(line) && int.TryParse($"{line[^1]}{Environment.NewLine}", out var count)
            ? count
            : 0;
    }
}