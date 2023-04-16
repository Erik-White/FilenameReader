namespace FilenameReader.Core
{
    public record FilePath(string FullPath)
    {
        public string Filename => Path.GetFileNameWithoutExtension(FullPath);

        public static FilePath Empty => new (string.Empty);
    }
}
