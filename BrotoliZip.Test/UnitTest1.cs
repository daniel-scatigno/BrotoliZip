using System.IO.Compression;
using System.Text;
using System.Text.Json;

namespace BrotoliZip.Test;

public class UnitTest1
{
    [Fact]
    public void CompressFiles()
    {
        var compressedFile = @"C:\Temp\BrotoliZip\BrotoliZip.Samples\compressed\compressed.brz";
        var directory = new DirectoryInfo(@"C:\Temp\BrotoliZip\BrotoliZip.Samples");

        // Brotli's highest compression level (11 = smallest size)

        var header = new Header() { Version = 1 };
        using (FileStream compressedFileStream = File.Create(compressedFile))
        {
            using (var brotoliStream = new BrotliStream(compressedFileStream, new BrotliCompressionOptions() { Quality = 11 },true))
            {
                long lastLength = 0;
                foreach (var file in directory.GetFiles())
                {
                    var compressedFileInfo = new Core.CompressedFileInfo() { Name = file.Name, Size = file.Length };

                    using (FileStream originalFileStream = file.OpenRead())
                    {
                        originalFileStream.CopyTo(brotoliStream);
                    }
                    brotoliStream.Flush();

                    compressedFileInfo.PackedSize = compressedFileStream.Length - lastLength;
                    header.Files.Add(compressedFileInfo);
                }
                brotoliStream.Flush();
            }
            string headerSerialized = JsonSerializer.Serialize(header);
            byte[] headerBytes = Encoding.UTF8.GetBytes(headerSerialized);


            compressedFileStream.Write(headerBytes);
        }

        Assert.True(File.Exists(compressedFile));


    }

    [Fact]
    public void Decompress()
    {

        string inputFile = @"C:\Temp\BrotoliZip\BrotoliZip.Samples\compressed\compressed.brz";
        DirectoryInfo output = new DirectoryInfo(@"C:\Temp\BrotoliZip\BrotoliZip.Samples\decompressed");

        using FileStream input = File.OpenRead(inputFile);
        using BrotliStream decompressionStream = new BrotliStream(input, CompressionMode.Decompress);
        

        byte[] bytes = new byte[698];
        input.Position = 19552385;
        input.ReadExactly(bytes);

        string headerJson = Encoding.UTF8.GetString(bytes);
        Header? header = JsonSerializer.Deserialize<Header>(headerJson);
        input.Position = 0;
        foreach (var file in header.Files)
        {
            using FileStream outputStream = File.Create(output.FullName + @"\" + file.Name);
            byte[] fileBytes = new byte[file.Size];
            decompressionStream.ReadExactly(fileBytes);
        }

        


        Console.WriteLine("Decompression completed.");

    }
}
