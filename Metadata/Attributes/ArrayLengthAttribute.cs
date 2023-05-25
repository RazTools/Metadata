namespace MetadataConverter2.Attributes;
[AttributeUsage(AttributeTargets.Field)]
internal class ArrayLengthAttribute : Attribute
{
    public int Length { get; set; }
}