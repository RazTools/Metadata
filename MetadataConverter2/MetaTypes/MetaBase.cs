namespace MetadataConverter2.MetaTypes
{
    public abstract record MetaBase
    {
        public string Name { get; set; }
        public MetaType Type { get; }
        public double Version { get; }
        public MetaBase(MetaType type, double version)
        {
            Name = type.ToString();
            Type = type;
            Version = version;
        }

        public abstract void Decrypt(MemoryStream stream);
        public abstract void Convert(MemoryStream stream);

        public sealed override string ToString()
        {
            return Name;
        }
    }

    public enum MetaType
    {
        GICB1,
        ZZZ,
        GI,
        GICBX,
        BH3,
        SR,
        GI_V2,
    }
}
