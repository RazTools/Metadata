using MetadataConverter2.Extensions;
using MetadataConverter2.IL2CPP;
using MetadataConverter2.Utils;
using static MetadataConverter2.IL2CPP.UnityIl2Cpp;

namespace MetadataConverter2.Converters;
public static class BlocksConverter
{
    public static void Convert(MemoryStream stream, double version, MetadataUsagePair[] metadataUsagePairs = null, MetadataUsageList[] metadataUsageList = null)
    {
        using BinaryStream bs = new(stream) { Version = version };
        using MemoryStream ms = new(0x1000);
        using BinaryStream newBS = new(ms) { Version = version };

        MhyIl2Cpp.GlobalMetadataHeader header = bs.ReadClass<MhyIl2Cpp.GlobalMetadataHeader>(0);
        GlobalMetadataHeader newHeader = new()
        {
            sanity = 0xFAB11BAF,
            version = (int)newBS.Version
        };

        newBS.Position = (uint)typeof(GlobalMetadataHeader).SizeOf(newBS.Version);

        bs.ConvertMetadataSection<MhyIl2Cpp.StringLiteral, StringLiteral>(newBS, header.stringLiteralOffset, header.stringLiteralSize, out newHeader.stringLiteralOffset, out newHeader.stringLiteralSize, ConvertersUtils.StringLiteralConverter);
        bs.CopyMetadataSection(newBS, header.stringLiteralDataOffset, header.stringLiteralDataSize, out newHeader.stringLiteralDataOffset, out newHeader.stringLiteralDataSize);
        bs.CopyMetadataSection(newBS, header.stringOffset, header.stringSize, out newHeader.stringOffset, out newHeader.stringSize);
        bs.CopyMetadataSection(newBS, header.eventsOffset, header.eventsSize, out newHeader.eventsOffset, out newHeader.eventsSize);
        bs.ConvertMetadataSection<MhyIl2Cpp.PropertyDefinition, PropertyDefinition>(newBS, header.propertiesOffset, header.propertiesSize, out newHeader.propertiesOffset, out newHeader.propertiesSize, ConvertersUtils.PropertyDefinitionConverter);
        bs.ConvertMetadataSection<MhyIl2Cpp.MethodDefinition, MethodDefinition>(newBS, header.methodsOffset, header.methodsSize, out newHeader.methodsOffset, out newHeader.methodsSize, ConvertersUtils.MethodDefinitionConverter);
        bs.CopyMetadataSection(newBS, header.parameterDefaultValuesOffset, header.parameterDefaultValuesSize, out newHeader.parameterDefaultValuesOffset, out newHeader.parameterDefaultValuesSize);
        bs.CopyMetadataSection(newBS, header.fieldDefaultValuesOffset, header.fieldDefaultValuesSize, out newHeader.fieldDefaultValuesOffset, out newHeader.fieldDefaultValuesSize);
        bs.CopyMetadataSection(newBS, header.fieldAndParameterDefaultValueDataOffset, header.fieldAndParameterDefaultValueDataSize, out newHeader.fieldAndParameterDefaultValueDataOffset, out newHeader.fieldAndParameterDefaultValueDataSize);
        bs.CopyMetadataSection(newBS, (int)header.fieldMarshaledSizesOffset, header.fieldMarshaledSizesSize, out newHeader.fieldMarshaledSizesOffset, out newHeader.fieldMarshaledSizesSize);
        bs.CopyMetadataSection(newBS, header.parametersOffset, header.parametersSize, out newHeader.parametersOffset, out newHeader.parametersSize);
        bs.ConvertMetadataSection<MhyIl2Cpp.FieldDefinition, FieldDefinition>(newBS, header.fieldsOffset, header.fieldsSize, out newHeader.fieldsOffset, out newHeader.fieldsSize, ConvertersUtils.FieldDefinitionConverter);
        bs.CopyMetadataSection(newBS, header.genericParametersOffset, header.genericParametersSize, out newHeader.genericParametersOffset, out newHeader.genericParametersSize);
        bs.CopyMetadataSection(newBS, header.genericParameterConstraintsOffset, header.genericParameterConstraintsSize, out newHeader.genericParameterConstraintsOffset, out newHeader.genericParameterConstraintsSize);
        bs.CopyMetadataSection(newBS, header.genericContainersOffset, header.genericContainersSize, out newHeader.genericContainersOffset, out newHeader.genericContainersSize);
        bs.CopyMetadataSection(newBS, header.nestedTypesOffset, header.nestedTypesSize, out newHeader.nestedTypesOffset, out newHeader.nestedTypesSize);
        bs.CopyMetadataSection(newBS, header.interfacesOffset, header.interfacesSize, out newHeader.interfacesOffset, out newHeader.interfacesSize);
        bs.CopyMetadataSection(newBS, header.vtableMethodsOffset, header.vtableMethodsSize, out newHeader.vtableMethodsOffset, out newHeader.vtableMethodsSize);
        bs.CopyMetadataSection(newBS, header.interfaceOffsetsOffset, header.interfaceOffsetsSize, out newHeader.interfaceOffsetsOffset, out newHeader.interfaceOffsetsSize);
        bs.ConvertMetadataSection<MhyIl2Cpp.TypeDefinition, TypeDefinition>(newBS, header.typeDefinitionsOffset, header.typeDefinitionsSize, out newHeader.typeDefinitionsOffset, out newHeader.typeDefinitionsSize, ConvertersUtils.TypeDefinitionConverter);
        if (newBS.Version < 24.5)
        {
            bs.CopyMetadataSection(newBS, header.rgctxEntriesOffset, header.rgctxEntriesCount, out newHeader.rgctxEntriesOffset, out newHeader.rgctxEntriesCount);
        }
        bs.CopyMetadataSection(newBS, header.imagesOffset, header.imagesSize, out newHeader.imagesOffset, out newHeader.imagesSize);
        bs.CopyMetadataSection(newBS, header.assembliesOffset, header.assembliesSize, out newHeader.assembliesOffset, out newHeader.assembliesSize);
        if (newBS.Version < 24.5)
        {
            bs.CopyMetadataSection(newBS, header.metadataUsageListsOffset, header.metadataUsageListsCount, out newHeader.metadataUsageListsOffset, out newHeader.metadataUsageListsCount);
            bs.CopyMetadataSection(newBS, header.metadataUsagePairsOffset, header.metadataUsagePairsCount, out newHeader.metadataUsagePairsOffset, out newHeader.metadataUsagePairsCount);
        }
        else
        {
            newBS.WriteMetadataSection(metadataUsageList, out newHeader.metadataUsageListsOffset, out newHeader.metadataUsageListsCount);
            newBS.WriteMetadataSection(metadataUsagePairs, out newHeader.metadataUsagePairsOffset, out newHeader.metadataUsagePairsCount);
        }
        bs.CopyMetadataSection(newBS, header.fieldRefsOffset, header.fieldRefsSize, out newHeader.fieldRefsOffset, out newHeader.fieldRefsSize);
        bs.CopyMetadataSection(newBS, header.referencedAssembliesOffset, header.referencedAssembliesSize, out newHeader.referencedAssembliesOffset, out newHeader.referencedAssembliesSize);
        bs.CopyMetadataSection(newBS, header.attributesInfoOffset, header.attributesInfoCount, out newHeader.attributesInfoOffset, out newHeader.attributesInfoCount);
        bs.CopyMetadataSection(newBS, header.attributeTypesOffset, header.attributeTypesCount, out newHeader.attributeTypesOffset, out newHeader.attributeTypesCount);
        bs.CopyMetadataSection(newBS, header.unresolvedVirtualCallParameterTypesOffset, header.unresolvedVirtualCallParameterTypesSize, out newHeader.unresolvedVirtualCallParameterTypesOffset, out newHeader.unresolvedVirtualCallParameterTypesSize);
        bs.CopyMetadataSection(newBS, header.unresolvedVirtualCallParameterRangesOffset, header.unresolvedVirtualCallParameterRangesSize, out newHeader.unresolvedVirtualCallParameterRangesOffset, out newHeader.unresolvedVirtualCallParameterRangesSize);
        bs.CopyMetadataSection(newBS, header.windowsRuntimeTypeNamesOffset, header.windowsRuntimeTypeNamesSize, out newHeader.windowsRuntimeTypeNamesOffset, out newHeader.windowsRuntimeTypeNamesSize);
        bs.CopyMetadataSection(newBS, header.exportedTypeDefinitionsOffset, header.exportedTypeDefinitionsSize, out newHeader.exportedTypeDefinitionsOffset, out newHeader.exportedTypeDefinitionsSize);

        newBS.WriteClass(0, newHeader);

        ms.MoveTo(stream);
    }
}