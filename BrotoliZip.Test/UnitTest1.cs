using System.IO.Compression;
using System.Text;
using System.Text.Json;
using BrotoliZip.Core;

namespace BrotoliZip.Test;

public class UnitTest1
{
    [Fact]
    public void CompressFiles()
    {
        var compressedFile = @"C:\Temp\BrotoliZip\BrotoliZip.Samples\compressed\compressed.brz";
        var directory = new DirectoryInfo(@"C:\Temp\BrotoliZip\BrotoliZip.Samples");
        var files = directory.GetFiles();
        new Compressor().CompressFiles(files.ToList(), compressedFile);
        Assert.True(File.Exists(compressedFile));


    }

    [Fact]
    public void Decompress()
    {

        string inputFile = @"C:\Temp\BrotoliZip\BrotoliZip.Samples\compressed\compressed.brz";
        DirectoryInfo output = new DirectoryInfo(@"C:\Temp\BrotoliZip\BrotoliZip.Samples\decompressed");

        using FileStream input = File.OpenRead(inputFile);
        using BrotliStream decompressionStream = new BrotliStream(input, CompressionMode.Decompress);

        var stringSignature = "BrotoliZipSignature";
        byte[] signature = Encoding.ASCII.GetBytes(stringSignature);
        input.Seek(-signature.Length - 4, SeekOrigin.End);

        byte[] footer = new byte[signature.Length + 4];
        input.ReadExactly(footer, 0, signature.Length + 4);
        if (Encoding.ASCII.GetString(footer, 0, signature.Length) != stringSignature)
            throw new InvalidDataException("File signature not found");

        int headerLength = BitConverter.ToInt32(footer, signature.Length);

        input.Seek(-(signature.Length + 4) - headerLength, SeekOrigin.End);

        byte[] headerBytes = new byte[headerLength];
        input.ReadExactly(headerBytes, 0, headerLength);        

        string headerJson = Encoding.UTF8.GetString(headerBytes);
        Header? header = JsonSerializer.Deserialize<Header>(headerJson);
        input.Position = 0;
        foreach (var file in header.Files)
        {
            using FileStream outputStream = File.Create(output.FullName + @"\" + file.Name);
            byte[] fileBytes = new byte[file.Size];
            decompressionStream.ReadExactly(fileBytes);
            outputStream.Write(fileBytes);
        }




        Console.WriteLine("Decompression completed.");

    }
}
