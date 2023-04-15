using System;
using System.IO;
using System.Text;
namespace ConsoleApp1
{
    class Program
    {
        public static string[] args;
        Program(string[] args)
        {
            Program.args = args;
        }
        private void Run()
        {
            var f = File.Open(args[0], FileMode.Open);
            int pos = args[0].IndexOf('.');
            string name = args[0].Substring(0, pos);
            System.IO.StreamReader file = new System.IO.StreamReader(f);
            string line;
            int counter = 0;
            while (true)
            {
                line = file.ReadLine();
                if (line == null) break;
                if (line.Contains(name))
                    counter++;
            }
            Console.WriteLine("found " + counter);
        }
        static void Main(string[] args)
        {
            Program program = new Program(args);
            program.Run();
        }
    }
}