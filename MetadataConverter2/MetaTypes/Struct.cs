using MetadataConverter2.Converters;

namespace MetadataConverter2.MetaTypes
{
    public record Struct : MetaBase
    {
        public Struct(MetaType type, double version) : base(type, version) { }

        public override void Convert(MemoryStream stream)
        {
            StructConverter.Convert(stream, Version, 24.5);
        }

        public override void Decrypt(MemoryStream stream) { }
    }
}
