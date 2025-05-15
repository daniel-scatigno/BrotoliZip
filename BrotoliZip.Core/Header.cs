using BrotoliZip.Core;

public class Header
{
    public decimal Version { get; set; } = 1;

    public List<CompressedFileInfo> Files { get; set; } = new();


    
}
