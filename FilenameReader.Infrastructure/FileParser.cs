using System.Text.RegularExpressions;

namespace FilenameReader.Infrastructure;

public class FileParser
{
    public int CountFileContents(string path)
    {
        var filename = Path.GetFileNameWithoutExtension(path);
        if (string.IsNullOrEmpty(filename))
        {
            throw new ArgumentOutOfRangeException(nameof(path));
        }

        using var f = File.Open(path, FileMode.Open);

        return ReadLines(f)
            .Where(line => !string.IsNullOrEmpty(line))
            .Sum(line => Regex.Matches(line, Regex.Escape(filename)).Count);
    }

    internal static IEnumerable<string?> ReadLines(FileStream fileStream)
    {
        using var file = new StreamReader(fileStream);

        do
        {
            yield return file.ReadLine();
        } while (!file.EndOfStream);
    }
}