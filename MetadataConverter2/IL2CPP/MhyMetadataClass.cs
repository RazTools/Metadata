using MetadataConverter2.Attributes;

namespace MetadataConverter2.IL2CPP;
public static class MhyIl2Cpp
{
    public class GlobalMetadataHeader
    {
        public uint sanity; // 0x00
        public int version;
        public uint seedPart50; // 0x08
        public uint seedPart51;
        public uint seedPart52; // 0x10
        public uint seedPart53;
        public uint stringLiteralDataOffset; // 0x18
        public int stringLiteralDataSize;
        public uint stringLiteralOffset; // 0x20
        public int stringLiteralSize;
        public uint genericContainersOffset; // 0x28
        public int genericContainersSize;
        public uint nestedTypesOffset; // 0x30
        public int nestedTypesSize;
        public uint interfacesOffset; // 0x38
        public int interfacesSize;
        public uint vtableMethodsOffset; // 0x40
        public int vtableMethodsSize;
        public int interfaceOffsetsOffset; // 0x48
        public int interfaceOffsetsSize;
        public uint typeDefinitionsOffset; // 0x50
        public int typeDefinitionsSize;
        [Version(Max = 24.1)]
        public uint rgctxEntriesOffset; // 0x58
        [Version(Max = 24.1)]
        public int rgctxEntriesCount;
        [Version(Max = 24.5)]
        public uint unk58; // 0x58
        [Version(Max = 24.5)]
        public int unk60;
        public uint seedPart10; //0x60
        public uint seedPart11;
        public uint seedPart12; //0x68
        public uint seedPart13;
        public uint imagesOffset; // 0x70
        public int imagesSize;
        public uint assembliesOffset; // 0x78
        public int assembliesSize;
        public uint fieldsOffset; // 0x80
        public int fieldsSize;
        public uint genericParametersOffset; // 0x88
        public int genericParametersSize;
        public uint fieldAndParameterDefaultValueDataOffset; // 0x90
        public int fieldAndParameterDefaultValueDataSize;
        public uint fieldMarshaledSizesOffset; // 0x98
        public int fieldMarshaledSizesSize;
        [Version(Min = 20)]
        public int referencedAssembliesOffset; // 0xA0
        [Version(Min = 20)]
        public int referencedAssembliesSize;
        [Version(Min = 21, Max = 27.2)]
        public uint attributesInfoOffset; // 0xA8
        [Version(Min = 21, Max = 27.2)]
        public int attributesInfoCount;
        [Version(Min = 21, Max = 27.2)]
        public uint attributeTypesOffset; // 0xB0
        [Version(Min = 21, Max = 27.2)]
        public int attributeTypesCount;
        [Version(Min = 22)]
        public int unresolvedVirtualCallParameterTypesOffset; // 0xB8
        [Version(Min = 22)]
        public int unresolvedVirtualCallParameterTypesSize;
        [Version(Min = 22)]
        public int unresolvedVirtualCallParameterRangesOffset; // 0xC0
        [Version(Min = 22)]
        public int unresolvedVirtualCallParameterRangesSize;
        [Version(Min = 23)]
        public int windowsRuntimeTypeNamesOffset; // 0xC8
        [Version(Min = 23)]
        public int windowsRuntimeTypeNamesSize;
        [Version(Min = 24)]
        public int exportedTypeDefinitionsOffset; // 0xD0
        [Version(Min = 24)]
        public int exportedTypeDefinitionsSize;
        public uint stringOffset; // 0xD8
        public int stringSize;
        public uint parametersOffset; // 0xE0
        public int parametersSize;
        public uint genericParameterConstraintsOffset; // 0xE8
        public int genericParameterConstraintsSize;
        public uint seedPart40; // 0xF0
        public uint seedPart41;
        [Version(Min = 19, Max = 24.5)]
        public uint metadataUsagePairsOffset; // 0xF8
        [Version(Min = 19, Max = 24.5)]
        public int metadataUsagePairsCount;
        public uint seedPart30; // 0x100
        public uint seedPart31;
        public uint seedPart32; // 0x108
        public uint seedPart33;
        [Version(Min = 19)]
        public uint fieldRefsOffset; // 0x110
        [Version(Min = 19)]
        public int fieldRefsSize;
        public uint eventsOffset; // 0x118
        public int eventsSize;
        public uint propertiesOffset; // 0x120
        public int propertiesSize;
        public uint methodsOffset; // 0x128
        public int methodsSize;
        public uint parameterDefaultValuesOffset; // 0x130
        public int parameterDefaultValuesSize;
        public uint fieldDefaultValuesOffset; // 0x138
        public int fieldDefaultValuesSize;
        public uint seedPart20; // 0x140
        public uint seedPart21;
        public uint seedPart22; // 0x148
        public uint seedPart23;
        [Version(Min = 19, Max = 24.5)]
        public uint metadataUsageListsOffset; // 0x150
        [Version(Min = 19, Max = 24.5)]
        public int metadataUsageListsCount;
    }

    public class TypeDefinition
    {
        public uint nameIndex;
        public uint namespaceIndex;
        [Version(Max = 24)]
        public int customAttributeIndex;
        public int byvalTypeIndex;
        [Version(Max = 24.5)]
        public int byrefTypeIndex;

        public int declaringTypeIndex;
        public int parentIndex;
        public int elementTypeIndex; // we can probably remove this one. Only used for enums

        [Version(Max = 24.1)]
        public int rgctxStartIndex;
        [Version(Max = 24.1)]
        public int rgctxCount;

        public int genericContainerIndex;

        public uint flags;

        public int fieldStart;
        public int propertyStart;
        public int methodStart;
        public int eventStart;
        public int nestedTypesStart;
        public int interfacesStart;
        public int interfaceOffsetsStart;
        public int vtableStart;

        public ushort event_count;
        public ushort method_count;
        public ushort property_count;
        public ushort field_count;
        public ushort vtable_count;
        public ushort interfaces_count;
        public ushort interface_offsets_count;
        public ushort nested_type_count;

        // bitfield to portably encode boolean values as single bits
        // 01 - valuetype;
        // 02 - enumtype;
        // 03 - has_finalize;
        // 04 - has_cctor;
        // 05 - is_blittable;
        // 06 - is_import_or_windows_runtime;
        // 07-10 - One of nine possible PackingSize values (0, 1, 2, 4, 8, 16, 32, 64, or 128)
        // 11 - PackingSize is default
        // 12 - ClassSize is default
        // 13-16 - One of nine possible PackingSize values (0, 1, 2, 4, 8, 16, 32, 64, or 128) - the specified packing size (even for explicit layouts)
        public uint bitfield;
        [Version(Min = 19)]
        public uint token;

        public bool IsValueType => (bitfield & 0x1) == 1;
        public bool IsEnum => ((bitfield >> 1) & 0x1) == 1;
    }

    public class MethodDefinition
    {
        public int returnType;
        public int declaringType;
        public uint unk08;
        public uint nameIndex;
        public int parameterStart;
        public int genericContainerIndex;
        [Version(Max = 24)]
        public int customAttributeIndex;
        [Version(Max = 24.1)]
        public int delegateWrapperIndex;
        public uint unk20;
        [Version(Max = 24.1)]
        public int methodIndex;
        [Version(Max = 24.1)]
        public int invokerIndex;
        [Version(Max = 24.1)]
        public int rgctxCount;
        [Version(Max = 24.1)]
        public int rgctxStartIndex;
        public ushort parameterCount;
        public ushort flags;
        public ushort slot;
        public ushort iflags;
        public uint token;
    }

    public class FieldDefinition
    {
        [Version(Max = 24)]
        public int customAttributeIndex;
        public int typeIndex;
        public uint nameIndex;
        [Version(Min = 19)]
        public uint token;
    }

    public class PropertyDefinition
    {
        [Version(Max = 24)]
        public int customAttributeIndex;
        public uint nameIndex;
        public uint unk08;
        [Version(Min = 19)]
        public uint token;
        public uint attrs;
        public uint unk14;
        public int set;
        public int get;
    }

    public class StringLiteral
    {
        public int dataIndex;
        public uint length;
    }

    public class MhyUsages
    {
        public ulong typeInfoUsageCount;
        public ulong typeInfoUsage;
        public ulong methodDefRefUsageCount;
        public ulong methodDefRefUsage;
        public ulong fieldInfoUsageCount;
        public ulong fieldInfoUsage;
        public ulong stringLiteralUsageCount;
        public ulong stringLiteralUsage;

        public ulong GetAddress(uint usage)
        {
            return usage switch
            {
                1 => typeInfoUsage,
                4 => fieldInfoUsage,
                5 => stringLiteralUsage,
                3 or 6 => methodDefRefUsage,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
