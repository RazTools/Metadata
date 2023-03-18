namespace MetadataConverter2.Extensions;
public static class MemoryStreamExtensions
{
    public static void MoveTo(this MemoryStream source, MemoryStream destination)
    {
        source.Position = 0;
        destination.Position = 0;
        source.CopyTo(destination);
        destination.SetLength(source.Length);
    }
}