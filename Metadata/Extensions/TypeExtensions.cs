using MetadataConverter2.Attributes;
using System.Reflection;

namespace MetadataConverter2.Extensions;
public static class TypeExtensions
{
    public static int SizeOf(this Type type, double version)
    {
        int size = 0;
        foreach (FieldInfo i in type.GetFields())
        {
            VersionAttribute? attr = (VersionAttribute)Attribute.GetCustomAttribute(i, typeof(VersionAttribute));
            if (attr != null)
            {
                if (version < attr.Min || version > attr.Max)
                {
                    continue;
                }
            }
            Type fieldType = i.FieldType;
            if (fieldType.IsPrimitive)
            {
                size += GetPrimitiveTypeSize(fieldType.Name);
            }
            else if (fieldType.IsEnum)
            {
                Type e = fieldType.GetField("value__").FieldType;
                size += GetPrimitiveTypeSize(e.Name);
            }
            else if (fieldType.IsArray)
            {
                ArrayLengthAttribute? arrayLengthAttribute = i.GetCustomAttribute<ArrayLengthAttribute>();
                size += arrayLengthAttribute.Length;
            }
            else
            {
                size += fieldType.SizeOf(version);
            }
        }
        return size;
    }
    private static int GetPrimitiveTypeSize(string name)
    {
        return name switch
        {
            "Int64" or "UInt64" => 8,
            "Int32" or "UInt32" => 4,
            "Int16" or "UInt16" => 2,
            _ => 0,
        };
    }
}
