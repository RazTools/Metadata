using MetadataConverter2.Attributes;
using MetadataConverter2.Extensions;
using System.Reflection;
using System.Text;

namespace MetadataConverter2.Utils;
public class BinaryStream : IDisposable
{
    public double Version;
    public bool Is32Bit;
    public ulong ImageBase;
    private readonly Stream stream;
    private readonly BinaryReader reader;
    private readonly BinaryWriter writer;
    private readonly MethodInfo readClass;
    private readonly MethodInfo readClassArray;
    private readonly MethodInfo writeClass;
    private readonly MethodInfo writeClassArray;
    private readonly Dictionary<Type, MethodInfo> genericMethodCache = new();
    private readonly Dictionary<FieldInfo, VersionAttribute[]> attributeCache = new();

    public BinaryStream(Stream input)
    {
        stream = input;
        reader = new BinaryReader(stream, Encoding.UTF8, true);
        writer = new BinaryWriter(stream, Encoding.UTF8, true);
        readClass = GetType().GetMethod("ReadClass", Type.EmptyTypes);
        readClassArray = GetType().GetMethod("ReadClassArray", new[] { typeof(long) });
        writeClass = GetType().GetMethod("WriteClass", Type.EmptyTypes);
        writeClassArray = GetType().GetMethod("WriteClassArray", new[] { typeof(long) });
    }

    public int Read(byte[] buffer, int index, int count)
    {
        return reader.Read(buffer, index, count);
    }

    public byte ReadByte()
    {
        return reader.ReadByte();
    }

    public byte[] ReadBytes(int count)
    {
        return reader.ReadBytes(count);
    }

    public short ReadInt16()
    {
        return reader.ReadInt16();
    }

    public ushort ReadUInt16()
    {
        return reader.ReadUInt16();
    }

    public int ReadInt32()
    {
        return reader.ReadInt32();
    }

    public uint ReadUInt32()
    {
        return reader.ReadUInt32();
    }

    public long ReadInt64()
    {
        return reader.ReadInt64();
    }

    public ulong ReadUInt64()
    {
        return reader.ReadUInt64();
    }

    public void Write(byte[] buffer, int index, int count)
    {
        writer.Write(buffer, index, count);
    }

    public void Write(byte value)
    {
        writer.Write(value);
    }

    public void Write(short value)
    {
        writer.Write(value);
    }

    public void Write(ushort value)
    {
        writer.Write(value);
    }

    public void Write(int value)
    {
        writer.Write(value);
    }

    public void Write(uint value)
    {
        writer.Write(value);
    }

    public void Write(long value)
    {
        writer.Write(value);
    }

    public void Write(ulong value)
    {
        writer.Write(value);
    }

    public void Write(byte[] value)
    {
        writer.Write(value);
    }

    public ulong Position
    {
        get => (ulong)stream.Position;
        set => stream.Position = (long)value;
    }

    public ulong Length => (ulong)stream.Length;

    public string ReadStringToNull(ulong addr, int count = 0x7FFF)
    {
        Position = addr;
        List<byte> bytes = new();
        int i = 0;
        while (i < count && Position < Length)
        {
            byte b = ReadByte();
            if (b == 0)
            {
                break;
            }

            bytes.Add(b);
            i++;
        }
        return Encoding.UTF8.GetString(bytes.ToArray());
    }
    public long ReadIntPtr()
    {
        return Is32Bit ? ReadInt32() : ReadInt64();
    }

    public virtual ulong ReadUIntPtr()
    {
        return Is32Bit ? ReadUInt32() : ReadUInt64();
    }

    public ulong PointerSize => Is32Bit ? 4ul : 8ul;

    private object ReadPrimitive(Type type)
    {
        string typename = type.Name;
        return typename switch
        {
            "Int32" => ReadInt32(),
            "UInt32" => ReadUInt32(),
            "Int16" => ReadInt16(),
            "UInt16" => ReadUInt16(),
            "Byte" => ReadByte(),
            "Int64" => ReadInt64(),
            "UInt64" => ReadUInt64(),
            _ => throw new NotSupportedException(),
        };
    }

    private void WritePrimitive(Type type, object value)
    {
        string typename = type.Name;
        switch (typename)
        {
            case "Int32":
                Write((int)value);
                break;
            case "UInt32":
                Write((uint)value);
                break;
            case "Int16":
                Write((short)value);
                break;
            case "UInt16":
                Write((ushort)value);
                break;
            case "Byte":
                Write((byte)value);
                break;
            case "Int64":
                Write((long)value);
                break;
            case "UInt64":
                Write((ulong)value);
                break;
            default:
                throw new NotSupportedException();
        }
    }

    public T ReadClass<T>(ulong addr) where T : new()
    {
        Position = addr;
        return ReadClass<T>();
    }

    public T ReadClass<T>() where T : new()
    {
        Type type = typeof(T);
        if (type.IsPrimitive)
        {
            return (T)ReadPrimitive(type);
        }
        else
        {
            T t = new();
            foreach (FieldInfo i in t.GetType().GetFields())
            {
                if (!attributeCache.TryGetValue(i, out VersionAttribute[]? versionAttributes))
                {
                    if (Attribute.IsDefined(i, typeof(VersionAttribute)))
                    {
                        versionAttributes = i.GetCustomAttributes<VersionAttribute>().ToArray();
                        attributeCache.Add(i, versionAttributes);
                    }
                }
                if (versionAttributes?.Length > 0)
                {
                    bool read = false;
                    foreach (VersionAttribute versionAttribute in versionAttributes)
                    {
                        if (Version >= versionAttribute.Min && Version <= versionAttribute.Max)
                        {
                            read = true;
                            break;
                        }
                    }
                    if (!read)
                    {
                        continue;
                    }
                }
                Type fieldType = i.FieldType;
                if (fieldType.IsPrimitive)
                {
                    i.SetValue(t, ReadPrimitive(fieldType));
                }
                else if (fieldType.IsEnum)
                {
                    Type e = fieldType.GetField("value__").FieldType;
                    i.SetValue(t, ReadPrimitive(e));
                }
                else if (fieldType.IsArray)
                {
                    ArrayLengthAttribute? arrayLengthAttribute = i.GetCustomAttribute<ArrayLengthAttribute>();
                    if (!genericMethodCache.TryGetValue(fieldType, out MethodInfo? methodInfo))
                    {
                        methodInfo = readClassArray.MakeGenericMethod(fieldType.GetElementType());
                        genericMethodCache.Add(fieldType, methodInfo);
                    }
                    i.SetValue(t, methodInfo.Invoke(this, new object[] { arrayLengthAttribute.Length }));
                }
                else
                {
                    if (!genericMethodCache.TryGetValue(fieldType, out MethodInfo? methodInfo))
                    {
                        methodInfo = readClass.MakeGenericMethod(fieldType);
                        genericMethodCache.Add(fieldType, methodInfo);
                    }
                    i.SetValue(t, methodInfo.Invoke(this, null));
                }
            }
            return t;
        }
    }

    public T[] ReadClassArray<T>(long count) where T : new()
    {
        T[] t = new T[count];
        for (int i = 0; i < count; i++)
        {
            t[i] = ReadClass<T>();
        }
        return t;
    }

    public T[] ReadClassArray<T>(ulong addr, long count) where T : new()
    {
        Position = addr;
        return ReadClassArray<T>(count);
    }

    public void WriteClass<T>(ulong addr, T value) where T : new()
    {
        Position = addr;
        WriteClass(value);
    }

    public void WriteClass<T>(T value) where T : new()
    {
        Type type = typeof(T);
        if (type.IsPrimitive)
        {
            WritePrimitive(type, value);
        }
        else
        {
            foreach (FieldInfo i in value.GetType().GetFields())
            {
                if (!attributeCache.TryGetValue(i, out VersionAttribute[]? versionAttributes))
                {
                    if (Attribute.IsDefined(i, typeof(VersionAttribute)))
                    {
                        versionAttributes = i.GetCustomAttributes<VersionAttribute>().ToArray();
                        attributeCache.Add(i, versionAttributes);
                    }
                }
                if (versionAttributes?.Length > 0)
                {
                    bool write = false;
                    foreach (VersionAttribute versionAttribute in versionAttributes)
                    {
                        if (Version >= versionAttribute.Min && Version <= versionAttribute.Max)
                        {
                            write = true;
                            break;
                        }
                    }
                    if (!write)
                    {
                        continue;
                    }
                }
                Type fieldType = i.FieldType;
                if (fieldType.IsPrimitive)
                {
                    WritePrimitive(fieldType, i.GetValue(value));
                }
                else if (fieldType.IsEnum)
                {
                    Type e = fieldType.GetField("value__").FieldType;
                    WritePrimitive(e, i.GetValue(value));
                }
                else if (fieldType.IsArray)
                {
                    ArrayLengthAttribute? arrayLengthAttribute = i.GetCustomAttribute<ArrayLengthAttribute>();
                    if (!genericMethodCache.TryGetValue(fieldType, out MethodInfo? methodInfo))
                    {
                        methodInfo = writeClassArray.MakeGenericMethod(fieldType.GetElementType());
                        genericMethodCache.Add(fieldType, methodInfo);
                    }
                    _ = methodInfo.Invoke(this, new object[] { arrayLengthAttribute.Length, i.GetValue(value) });
                }
                else
                {
                    if (!genericMethodCache.TryGetValue(fieldType, out MethodInfo? methodInfo))
                    {
                        methodInfo = writeClass.MakeGenericMethod(fieldType);
                        genericMethodCache.Add(fieldType, methodInfo);
                    }
                    _ = methodInfo.Invoke(this, new object[] { i.GetValue(value) });
                }
            }
        }
    }

    public void WriteClassArray<T>(long count, T[] values) where T : new()
    {
        for (int i = 0; i < count; i++)
        {
            T? value = values[i];
            WriteClass(value);
        }
    }

    public void WriteClassArray<T>(ulong addr, long count, T[] values) where T : new()
    {
        Position = addr;
        WriteClassArray(count, values);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            reader.Close();
            writer.Close();
            stream.Close();
        }
    }

    public void Dispose()
    {
        Dispose(true);
    }
}