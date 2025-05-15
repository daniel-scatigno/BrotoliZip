using System.IO.Compression;
using System.Text;

namespace BrotoliZip.Core;

public class Compressor
{
    public const string textSignature = "BrotoliZipSignature";
    public virtual void CompressFiles(List<FileInfo> files, string destination, int quality = 11)
    {

        var header = new Header() { Version = 1 };
        using (FileStream compressedFileStream = File.Create(destination))
        {
            using (var brotoliStream = new BrotliStream(compressedFileStream, new BrotliCompressionOptions() { Quality = quality }, true))
            {
                long lastLength = 0;
                foreach (var file in files)
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

            string headerSerialized = System.Text.Json.JsonSerializer.Serialize(header);
            byte[] headerBytes = Encoding.UTF8.GetBytes(headerSerialized);
            byte[] signature = Encoding.ASCII.GetBytes(textSignature);
            byte[] headerLengthBytes = BitConverter.GetBytes(headerBytes.Length);

            compressedFileStream.Write(headerBytes);
            compressedFileStream.Write(signature);
            compressedFileStream.Write(headerLengthBytes);
        }

    }

}
