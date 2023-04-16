using FilenameReader.Core;

namespace FilenameReader.Infrastructure
{
    public interface IFileParser
    {
        int CountFileContents(FilePath filePath);
    }
}