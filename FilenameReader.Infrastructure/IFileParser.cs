using FilenameReader.Core;
using FilenameReader.Infrastructure.Validators;
using OneOf;
using OneOf.Types;

namespace FilenameReader.Infrastructure
{
    public interface IFileParser
    {
        OneOf<int, ValidationFailed, NotFound, Error<Exception>> CountFileContents(FilePath filePath, bool ignoreCase = false);
    }
}