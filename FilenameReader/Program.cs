using FilenameReader.Infrastructure;

namespace FilenameReaderCli
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var filepath = args.FirstOrDefault();
            if (string.IsNullOrEmpty(filepath))
            {
                Console.WriteLine("A file name or path must be supplied.");
                return;
            }

            var parser = new FileParser();

            try
            {
                var count = parser.CountFileContents(filepath);

                Console.WriteLine(count);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}