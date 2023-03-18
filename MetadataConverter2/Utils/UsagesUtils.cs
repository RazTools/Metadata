using AsmResolver;
using AsmResolver.PE.File;
using AsmResolver.PE.File.Headers;
using Iced.Intel;
using Reloaded.Memory.Sigscan;
using System.Runtime.InteropServices;
using static MetadataConverter2.IL2CPP.MhyIl2Cpp;

namespace MetadataConverter2.Utils
{
    public static class UsagesUtils
    {
        private const string Signature = "4C 8D 0D ?? ?? ?? ?? 4C 8D 05 ?? ?? ?? ?? 48 8D 15 ?? ?? ?? ?? 48 8D 0D ?? ?? ?? ?? E9 ?? ?? ?? ??";

        public static bool FindUsagesRVA(this PEFile peFile, out uint mhyUsagesRVA)
        {
            mhyUsagesRVA = 0;

            PESection? section = peFile.Sections.FirstOrDefault(x => x.Name == ".text");
            if (section != default)
            {
                byte[] bytes = section.WriteIntoArray();

                Scanner scanner = new(bytes);
                Reloaded.Memory.Sigscan.Definitions.Structs.PatternScanResult result = scanner.FindPattern(Signature);

                if (result.Found)
                {
                    uint ip = peFile.FileOffsetToRva((uint)result.Offset + section.Offset);
                    List<Instruction> instructions = Decode(bytes, result.Offset, 0x1C, ip);

                    if (instructions.Count == 4)
                    {
                        mhyUsagesRVA = instructions[^3].MemoryDisplacement32;
                        return true;
                    }
                }
            }

            return false;
        }

        public static MhyUsages GetMhyUsages(this PEFile peFile, uint mhyUsagesRVA)
        {
            PESection section = peFile.GetSectionContainingRva(mhyUsagesRVA);
            byte[] bytes = section.WriteIntoArray();

            using MemoryStream ms = new(bytes);
            using BinaryStream bs = new(ms);
            return bs.ReadClass<MhyUsages>(mhyUsagesRVA - section.Rva);
        }

        public static void PatchMhyUsages(this PEFile peFile, uint mhyUsagesRVA, ulong[] metadataUsages)
        {
            byte[] metadataUsagesBytes = MemoryMarshal.AsBytes<ulong>(metadataUsages).ToArray();
            DataSegment metadataUsagesSegment = new(metadataUsagesBytes);

            PESection metadataUsagesSection = new(".usages", SectionFlags.Align8Bytes, metadataUsagesSegment);
            peFile.Sections.Add(metadataUsagesSection);
            peFile.UpdateHeaders();

            PESection section = peFile.GetSectionContainingRva(mhyUsagesRVA);

            using MemoryStream ms = new();
            using BinaryStream bs = new(ms);
            bs.Write((ulong)metadataUsages.Length);
            bs.Write(peFile.OptionalHeader.ImageBase + metadataUsagesSection.Rva);
            byte[] bytes = ms.ToArray();

            section.Contents = section.Contents.AsPatchedSegment().Patch(mhyUsagesRVA - section.Rva, bytes);
        }

        private static List<Instruction> Decode(byte[] bytes, int offset, int count, ulong ip)
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
}
