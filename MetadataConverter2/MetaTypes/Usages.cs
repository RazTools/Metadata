using AsmResolver.PE.File;
using MetadataConverter2.Converters;
using MetadataConverter2.IL2CPP;
using MetadataConverter2.Utils;

namespace MetadataConverter2.MetaTypes;
public record Usages : Blocks
{
    private string il2cpp_path = string.Empty;

    public Usages(MetaType type, byte[] initVector, double version) : base(type, initVector, version) { }
    public override void Convert(MemoryStream stream)
    {
        if (string.IsNullOrEmpty(il2cpp_path))
        {
            Console.WriteLine("Please enter path to Il2Cpp binary to be patched (UserAssembly.dll):");
            string path = @"" + Console.ReadLine();
            if (!File.Exists(path))
            {
                throw new FileNotFoundException();
            }

            il2cpp_path = path;
        }

        byte[] bytes = File.ReadAllBytes(il2cpp_path);
        PEFile peFile = PEFile.FromBytes(bytes);

        if (peFile.FindUsagesRVA(out uint mhyUsagesRVA))
        {
            Console.WriteLine($"Found mhyUsages at 0x{mhyUsagesRVA:X8} !!");

            MhyIl2Cpp.MhyUsages usages = peFile.GetMhyUsages(mhyUsagesRVA);

            Console.WriteLine("Applying mhyUsages...");
            UsagesConverter.Convert(stream, usages, Version, 24.5, out ulong[]? metadataUsages);

            Console.WriteLine($"Patching {Path.GetFileName(il2cpp_path)}...");
            string outputPath = Path.Combine(Path.GetDirectoryName(il2cpp_path), $"{Path.GetFileNameWithoutExtension(il2cpp_path)}_patched{Path.GetExtension(il2cpp_path)}");
            peFile.PatchMhyUsages(mhyUsagesRVA, metadataUsages);
            peFile.Write(outputPath);
            Console.WriteLine($"Generated patched file at {outputPath} !!");
        }
    }
}


