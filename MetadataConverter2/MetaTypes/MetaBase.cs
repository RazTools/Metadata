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

        public abstract bool Decrypt(MemoryStream stream);
        public abstract bool Convert(MemoryStream stream);

        public sealed override string ToString()
        {
            return Name;
        }
    }

    public enum MetaType
    {
        GI,
        GICB1,
        GICBX,
        GIV2,
        BH3Pre,
        BH3,
        BH3V2,
        SR,
        SRV2,
        ZZZ,
    }
}
