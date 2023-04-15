namespace FilenameReader.Cli.Test
{
    internal static class TestHelper
    {
        public const string TestDataDirectory = "TestData";

        public static string TestDataPath
            => Path.Combine(GetProjectBaseDirectory(), TestDataDirectory);

        private static string GetProjectBaseDirectory()
        {
            var pathItems = AppDomain.CurrentDomain.BaseDirectory.Split(Path.DirectorySeparatorChar);
            var pos = pathItems.Reverse().ToList().FindIndex(x => string.Equals("bin", x));

            return Path.Combine(pathItems.Take(pathItems.Length - pos - 1).ToArray());
        }
    }
}
