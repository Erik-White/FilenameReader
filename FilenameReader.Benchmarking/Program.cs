using BenchmarkDotNet.Running;
using FilenameReader.Infrastructure.Test;

namespace FilenameReader.Benchmarking;

public static class Program
{
    public static void Main()
    {
        BenchmarkRunner.Run<TextSearcherBenchmarks>();
    }
}
