namespace BrotoliZip.Core;

public class CompressedFileInfo
{
    public string Name { get; set; } = string.Empty;
    public long Size { get; set; } //Size in bytes
    public long PackedSize { get; set; }
    public DateTime Modified { get; set; }
    public long InitialBlock { get; set; }
    public long EndlBlock { get; set; }
    public string Attribute{ get; set; } = string.Empty;

}
