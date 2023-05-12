using MetadataConverter2.Converters;
using MetadataConverter2.Extensions;
using MetadataConverter2.IL2CPP;
using MetadataConverter2.Utils;
using static MetadataConverter2.IL2CPP.UnityIl2Cpp;

namespace MetadataConverter2.MetaTypes;
public record StructUsages : Usages
{
    public StructUsages(MetaType type, byte[] initVector, double version) : base(type, initVector, version) { }
    protected override void Apply(MemoryStream stream, MhyIl2Cpp.MhyUsages usages, out ulong[]? metadataUsages)
    {
        using BinaryStream bs = new(stream, true) { Version = Version };

        MhyIl2Cpp.GlobalMetadataHeader header = bs.ReadClass<MhyIl2Cpp.GlobalMetadataHeader>(0);

        var metadataUsagePairs = bs.ReadMetadataClassArray<MetadataUsagePair>(header.metadataUsagePairsOffset, header.metadataUsagePairsCount);
        var metadataUsageList = bs.ReadMetadataClassArray<MetadataUsageList>(header.metadataUsageListsOffset, header.metadataUsageListsCount);

        metadataUsages = new ulong[metadataUsagePairs.Length];
        foreach (MetadataUsagePair metadataUsagePair in metadataUsagePairs)
        {
            uint usage = GetEncodedIndexType(metadataUsagePair.encodedSourceIndex);
            uint decodedIndex = GetDecodedMethodIndex(metadataUsagePair.encodedSourceIndex);

            metadataUsages[metadataUsagePair.destinationIndex] = usages.GetAddress(usage) + (decodedIndex * 8);
        }

        StructConverter.Convert(stream, Version);
    }
}


