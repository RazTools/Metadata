using MetadataConverter2.Utils;

namespace MetadataConverter2.Extensions;
public static class BinaryStreamExtensions
{
    private const int BufferSize = 81920;

    public static void BlockCopyTo(this BinaryStream source, BinaryStream destination, int size)
    {
        byte[] buffer = new byte[BufferSize];
        for (int left = size; left > 0; left -= BufferSize)
        {
            int toRead = BufferSize < left ? BufferSize : left;
            int read = source.Read(buffer, 0, toRead);
            destination.Write(buffer, 0, read);
            if (read != toRead)
            {
                return;
            }
        }
    }
    public static T[] ReadMetadataClassArray<T>(this BinaryStream stream, uint addr, int count) where T : new()
    {
        return stream.ReadClassArray<T>(addr, count / typeof(T).SizeOf(stream.Version));
    }
    public static void WriteMetadataClassArray<T>(this BinaryStream stream, uint addr, int count, T[] values) where T : new()
    {
        stream.WriteClassArray(addr, count / typeof(T).SizeOf(stream.Version), values);
    }
    public static void WriteMetadataSection<T>(this BinaryStream stream, T[] values, out uint newAddr, out int newCount) where T : new()
    {
        newAddr = (uint)stream.Position;
        newCount = values.Length * typeof(T).SizeOf(stream.Version);
        stream.WriteMetadataClassArray(newAddr, newCount, values);
    }
    public static void ConvertMetadataSection<TFrom, TTo>(this BinaryStream stream, BinaryStream outStream, uint addr, int count, out uint newAddr, out int newCount, Func<TFrom, TTo> converter) where TFrom : new() where TTo : new()
    {
        TFrom[] values = stream.ReadMetadataClassArray<TFrom>(addr, count);

        TTo[] newValues = new TTo[values.Length];
        for (int i = 0; i < values.Length; i++)
        {
            newValues[i] = converter(values[i]);
        }

        outStream.WriteMetadataSection(newValues, out newAddr, out newCount);
    }
    public static void CopyMetadataSection(this BinaryStream stream, BinaryStream outStream, int addr, int count, out int newAddr, out int newCount)
    {
        CopyMetadataSection(stream, outStream, (uint)addr, count, out uint temp, out newCount);
        newAddr = (int)temp;
    }
    public static void CopyMetadataSection(this BinaryStream stream, BinaryStream outStream, uint addr, int count, out uint newAddr, out int newCount)
    {
        newCount = count;
        newAddr = (uint)outStream.Position;

        stream.Position = addr;
        stream.BlockCopyTo(outStream, count);
    }
}
