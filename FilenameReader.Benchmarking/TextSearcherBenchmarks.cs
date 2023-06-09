﻿using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Text;
using BenchmarkDotNet.Attributes;
using FilenameReader.Core;

namespace FilenameReader.Infrastructure.Test;

[SimpleJob(BenchmarkDotNet.Engines.RunStrategy.Throughput)]
public class TextSearcherBenchmarks
{
    private readonly IFileSystem _fileSystem;
    private readonly ITextSearcher _textSearcher;
    private readonly FilePath _filePath;

    public TextSearcherBenchmarks()
    {
        _fileSystem = new MockFileSystem();
        _textSearcher = new RegexTextSearcher();
        _filePath = new FilePath(Guid.NewGuid().ToString());
    }

    [GlobalSetup]
    public void Setup()
    {
        using var stream = _fileSystem.File.OpenWrite(_filePath.FullPath);

        // Create a 1GB file that contains the file name once at the end
        WriteRandomContent(stream, fileSizeMegabytes: 1024);
        var filePathBytes = Encoding.UTF8.GetBytes(_filePath.Filename);
        stream.Write(filePathBytes, 0, filePathBytes.Length);
    }

    [Benchmark(Baseline = true)]
    public int CountTextSyncronously()
    {
        using var stream = _fileSystem.File.OpenRead(_filePath.FullPath);

        return _textSearcher.CountStreamContents(stream, _filePath.Filename, new TextSearchOptions());
    }

    [Benchmark]
    public async Task<int> CountTextAsyncronously()
    {
        using var stream = _fileSystem.File.OpenRead(_filePath.FullPath);

        return await _textSearcher.CountStreamContentsAsync(stream, _filePath.Filename, new TextSearchOptions());
    }

    private static void WriteRandomContent(Stream stream, int fileSizeMegabytes)
    {
        const int BlockSize = 1024 * 8;
        const int BlocksPerMegabyte = 1024 * 1024 / BlockSize;

        var data = new byte[BlockSize];
        var random = new Random();

        for (int i = 0; i < fileSizeMegabytes * BlocksPerMegabyte; i++)
        {
            random.NextBytes(data);
            // Write a newline character to the end of each block
            data[^1] = 0x0A;
            stream.Write(data, 0, data.Length);
        }
    }
}
