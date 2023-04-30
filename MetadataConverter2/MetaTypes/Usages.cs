using AsmResolver.PE.File;
using MetadataConverter2.Extensions;
using MetadataConverter2.IL2CPP;

namespace MetadataConverter2.MetaTypes;
public abstract record Usages : Blocks
{
    protected string il2cpp_path = string.Empty;

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

        if (peFile.TryGetCodegenRegisteration(Version, out CodegenRegistration codegen))
        {
            Console.WriteLine($"Found codegen at 0x{peFile.GetVA(codegen.Rva):X8} !!");

            Console.WriteLine("Applying Usages...");
            Apply(stream, codegen.Usages, out var metadataUsages);

            var fileInfo = new FileInfo(il2cpp_path);
            string outputPath = Path.Combine(fileInfo.Directory.FullName, $"{Path.GetFileNameWithoutExtension(fileInfo.Name)}_patched{fileInfo.Extension}");

            Console.WriteLine($"Patching {fileInfo.Name}...");
            codegen.Patch(peFile, Version, metadataUsages);
            peFile.Write(outputPath);

            Console.WriteLine($"Generated patched file at {outputPath} !!");
        }
    }
    protected abstract void Apply(MemoryStream stream, MhyIl2Cpp.MhyUsages usages, out ulong[]? metadataUsages);
    protected uint GetEncodedIndexType(uint index)
    {
        return (index & 0xE0000000) >> 29;
    }

    protected uint GetDecodedMethodIndex(uint index)
    {
        return Version >= 27 ? (index & 0x1FFFFFFEU) >> 1 : index & 0x1FFFFFFFU;
    }
}


