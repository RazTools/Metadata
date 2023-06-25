using AsmResolver.PE.File;
using MetadataConverter2.Extensions;
using MetadataConverter2.IL2CPP;
using MetadataConverter2.Utils;
using static MetadataConverter2.IL2CPP.UnityIl2Cpp;

namespace MetadataConverter2.MetaTypes;
public record Usages : Blocks
{
    private string il2cpp_path = string.Empty;
    private Func<MemoryStream, double, MetadataUsagePair[], MetadataUsageList[], bool> Converter;

    private PEFile PEFile;
    private ulong[] metadataUsages;
    private CodegenRegistration Codegen;
    private MetadataUsageList[] metadataUsageLists;
    private MetadataUsagePair[] metadataUsagePairs;

    public Usages(MetaType type, byte[] initVector, double version, Func<MemoryStream, double, MetadataUsagePair[], MetadataUsageList[], bool> converter) : base(type, initVector, version)
    {
        Converter = converter;
    }
    public override bool Convert(MemoryStream stream)
    {
        if (string.IsNullOrEmpty(il2cpp_path))
        {
            Console.WriteLine("Please enter path to il2cpp binary to be patched:");
            string path = Console.ReadLine().Trim('\"');
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Unable to find file at given path", path);
            }

            il2cpp_path = path;
        }

        byte[] bytes = File.ReadAllBytes(il2cpp_path);
        PEFile = PEFile.FromBytes(bytes);

        if (!PEFile.TryGetCodegenRegisteration(Version, out Codegen))
        {
            Console.WriteLine("Unable to find codegen !!");
            return false;
        }

        Console.WriteLine($"Found codegen at 0x{PEFile.GetVA(Codegen.Rva):X8} !!");

        Console.WriteLine("Applying Usages...");

        ApplyUsages(stream);

        var fileInfo = new FileInfo(il2cpp_path);
        string outputPath = Path.Combine(fileInfo.Directory.FullName, $"{Path.GetFileNameWithoutExtension(fileInfo.Name)}_patched{fileInfo.Extension}");

        Console.WriteLine($"Patching {fileInfo.Name}...");
        Codegen.Patch(PEFile, Version, metadataUsages);
        PEFile.Write(outputPath);

        Console.WriteLine($"Generated patched il2cpp binary at {outputPath} !!");
        return true;
    }
    private void ApplyUsages(MemoryStream stream)
    {
        using BinaryStream bs = new(stream, true) { Version = Version };

        MhyIl2Cpp.GlobalMetadataHeader header = bs.ReadClass<MhyIl2Cpp.GlobalMetadataHeader>(0);

        metadataUsagePairs = bs.ReadMetadataClassArray<MetadataUsagePair>(header.metadataUsagePairsOffset, header.metadataUsagePairsCount);
        metadataUsageLists = new MetadataUsageList[] { new MetadataUsageList() { start = 0, count = (uint)metadataUsagePairs.Length } };

        uint i = 0;
        var usages = new Dictionary<ulong, uint>();
        foreach (var metadataUsagePair in metadataUsagePairs)
        {
            var usage = (MetadataUsage)GetEncodedIndexType(metadataUsagePair.encodedSourceIndex);
            
            var address = Codegen.Usages.GetAddress(usage, metadataUsagePair.destinationIndex);
            if (usages.TryGetValue(address, out var index))
            {
                metadataUsagePair.destinationIndex = index;
            }
            else
            {
                usages[address] = i;
                metadataUsagePair.destinationIndex = i++;
            }
        }

        metadataUsages = usages.Keys.ToArray();

        Converter(stream, Version, metadataUsagePairs, metadataUsageLists);
    }
    private static uint GetEncodedIndexType(uint index)
    {
        return (index & 0xE0000000) >> 29;
    }
}


