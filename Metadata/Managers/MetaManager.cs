using MetadataConverter2.MetaTypes;
using static MetadataConverter2.Crypto.CryptoHelper;

namespace MetadataConverter2.Managers;
public static class MetaManager
{
    private static readonly Dictionary<int, MetaBase> Metas = new();
    static MetaManager()
    {
        int index = 0;
        Metas.Add(index++, new Blocks(MetaType.GI, GIInitVector, 24));
        Metas.Add(index++, new Mark(MetaType.GICB1, 24));
        Metas.Add(index++, new Blocks(MetaType.GICBX, new byte[0x10], 24));
        Metas.Add(index++, new BlocksUsages(MetaType.GIV2, GIInitVector, 24.5));
        Metas.Add(index++, new Blocks(MetaType.BH3Pre, new byte[0x10], 24));
        Metas.Add(index++, new Blocks(MetaType.BH3, BH3InitVector, 24));
        Metas.Add(index++, new BlocksUsages(MetaType.BH3V2, BH3InitVector, 24.5));
        Metas.Add(index++, new Struct(MetaType.SR, SRInitVector, 24.5));  
        Metas.Add(index++, new StructUsages(MetaType.SRV2, SRInitVector, 24.5));
        Metas.Add(index++, new Struct(MetaType.ZZZ, Array.Empty<byte>(), 24.5));
    }
    public static MetaBase GetMeta(MetaType metaType)
    {
        return GetMeta((int)metaType);
    }

    public static MetaBase GetMeta(int index)
    {
        return !Metas.TryGetValue(index, out MetaBase? meta) ? throw new ArgumentException("Invalid meta !!") : meta;
    }

    public static MetaBase GetMeta(string name)
    {
        return Metas.FirstOrDefault(x => x.Value.Name == name).Value;
    }

    public static MetaBase[] GetMetas()
    {
        return Metas.Values.ToArray();
    }

    public static string[] GetMetaNames()
    {
        return Metas.Values.Select(x => x.Name).ToArray();
    }

    public static string SupportedMetas()
    {
        return $"Supported Metas:\n{string.Join("\n", Metas.Values.Select(x => x.Name))}";
    }
}