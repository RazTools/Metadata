namespace MetadataConverter2.Attributes;
[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public class VersionAttribute : Attribute
{
    public double Min { get; set; } = 0;
    public double Max { get; set; } = 99;
}
