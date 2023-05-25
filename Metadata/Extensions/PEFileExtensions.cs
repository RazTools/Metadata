using AsmResolver;
using AsmResolver.PE.File;
using AsmResolver.PE.File.Headers;
using Iced.Intel;
using MetadataConverter2.Utils;
using Reloaded.Memory.Sigscan;
using System.Runtime.InteropServices;
using static MetadataConverter2.IL2CPP.MhyIl2Cpp;
using static MetadataConverter2.IL2CPP.UnityIl2Cpp;

namespace MetadataConverter2.Extensions;
public static class PEFileExtensions
{
    private const string Signature = "4C 8D 0D ?? ?? ?? ?? 4C 8D 05 ?? ?? ?? ?? 48 8D 15 ?? ?? ?? ?? 48 8D 0D ?? ?? ?? ?? E9 ?? ?? ?? ??";
    public static ulong GetVA(this PEFile peFile, ulong rva)
    {
        return peFile.OptionalHeader.ImageBase + rva;
    }

    public static bool TryGetCodegenRegisteration(this PEFile peFile, double version, out CodegenRegistration codegen)
    {
        codegen = new CodegenRegistration();

        PESection? section = peFile.Sections.FirstOrDefault(x => x.Name == ".text");
        if (section != default)
        {
            byte[] bytes = section.WriteIntoArray();

            Scanner scanner = new(bytes);
            Reloaded.Memory.Sigscan.Definitions.Structs.PatternScanResult result = scanner.FindPattern(Signature);

            if (result.Found)
            {
                codegen.Rva = peFile.FileOffsetToRva((uint)result.Offset + section.Offset);
                List<Instruction> instructions = Decode(bytes, result.Offset, 0x1C, codegen.Rva);
                if (instructions.Count == 4)
                {
                    codegen.UsagesRVA = instructions[^3].MemoryDisplacement32;
                    codegen.MetadataRVA = instructions[^2].MemoryDisplacement32;

                    codegen.Usages = peFile.ReadClass<MhyUsages>(version, codegen.UsagesRVA);
                    codegen.Metadata = peFile.ReadClass<MetadataRegistration>(version, codegen.MetadataRVA);

                    return true;
                }
            }
        }

        return false;
    }

    public static T ReadClass<T>(this PEFile peFile, double version, uint rva) where T : new()
    {
        PESection section = peFile.GetSectionContainingRva(rva);
        byte[] bytes = section.WriteIntoArray();

        using MemoryStream ms = new(bytes);
        using BinaryStream bs = new(ms) { Version = version };
        return bs.ReadClass<T>(rva - section.Rva);
    }

    public static List<Instruction> Decode(byte[] bytes, int offset, int count, ulong ip)
    {
        List<Instruction> instructions = new();
        ByteArrayCodeReader codeReader = new(bytes, offset, count);
        Decoder decoder = Decoder.Create(64, codeReader, ip);

        while (codeReader.CanReadByte)
        {
            instructions.Add(decoder.Decode());
        }

        return instructions;
    }
}

public record CodegenRegistration
{
    public ulong Rva;
    public uint UsagesRVA;
    public uint MetadataRVA;
    public MhyUsages? Usages;
    public MetadataRegistration? Metadata;

    public void Patch(PEFile peFile, double version, ulong[] metadataUsages)
    {
        byte[] metadataUsagesBytes = MemoryMarshal.AsBytes<ulong>(metadataUsages).ToArray();
        PESection usagesSection = new(".usages", SectionFlags.ContentInitializedData | SectionFlags.MemoryRead, new DataSegment(metadataUsagesBytes));
        peFile.Sections.Add(usagesSection);
        peFile.UpdateHeaders();

        Metadata.metadataUsages = peFile.GetVA(usagesSection.Rva);
        Metadata.metadataUsagesCount = (ulong)metadataUsages.Length;

        using MemoryStream ms = new();
        using BinaryStream bs = new(ms) { Version = version };
        bs.WriteClass(Metadata);
        byte[] metadataBytes = ms.ToArray();

        PESection metadataSection = peFile.GetSectionContainingRva(MetadataRVA);
        uint relativeOffset = (uint)(peFile.RvaToFileOffset(MetadataRVA) - metadataSection.Offset);
        metadataSection.Contents = metadataSection.Contents.AsPatchedSegment().Patch(relativeOffset, metadataBytes);
    }
}