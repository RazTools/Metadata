using MetadataConverter2.Extensions;
using MetadataConverter2.IL2CPP;
using MetadataConverter2.Utils;

namespace MetadataConverter2.Converters;
public static class BlocksConverter
{
    public static void Convert(MemoryStream stream, double version, double targetVersion)
    {
        using BinaryStream bs = new(stream) { Version = version };
        using MemoryStream ms = new(0x1000);
        using BinaryStream newBS = new(ms) { Version = targetVersion };

        MhyIl2Cpp.GlobalMetadataHeader header = bs.ReadClass<MhyIl2Cpp.GlobalMetadataHeader>(0);
        UnityIl2Cpp.GlobalMetadataHeader newHeader = new()
        {
            sanity = 0xFAB11BAF,
            version = (int)newBS.Version
        };

        newBS.Position = (uint)newBS.SizeOf(typeof(UnityIl2Cpp.GlobalMetadataHeader));

        bs.ConvertMetadataSection<MhyIl2Cpp.StringLiteral, UnityIl2Cpp.StringLiteral>(newBS, header.stringLiteralOffset, header.stringLiteralSize, out newHeader.stringLiteralOffset, out newHeader.stringLiteralSize, StringLiteralConverter);
        bs.CopyMetadataSection(newBS, header.stringLiteralDataOffset, header.stringLiteralDataSize, out newHeader.stringLiteralDataOffset, out newHeader.stringLiteralDataSize);
        bs.CopyMetadataSection(newBS, header.stringOffset, header.stringSize, out newHeader.stringOffset, out newHeader.stringSize);
        bs.CopyMetadataSection(newBS, header.eventsOffset, header.eventsSize, out newHeader.eventsOffset, out newHeader.eventsSize);
        bs.ConvertMetadataSection<MhyIl2Cpp.PropertyDefinition, UnityIl2Cpp.PropertyDefinition>(newBS, header.propertiesOffset, header.propertiesSize, out newHeader.propertiesOffset, out newHeader.propertiesSize, PropertyDefinitionConverter);
        bs.ConvertMetadataSection<MhyIl2Cpp.MethodDefinition, UnityIl2Cpp.MethodDefinition>(newBS, header.methodsOffset, header.methodsSize, out newHeader.methodsOffset, out newHeader.methodsSize, MethodDefinitionConverter);
        bs.CopyMetadataSection(newBS, header.parameterDefaultValuesOffset, header.parameterDefaultValuesSize, out newHeader.parameterDefaultValuesOffset, out newHeader.parameterDefaultValuesSize);
        bs.CopyMetadataSection(newBS, header.fieldDefaultValuesOffset, header.fieldDefaultValuesSize, out newHeader.fieldDefaultValuesOffset, out newHeader.fieldDefaultValuesSize);
        bs.CopyMetadataSection(newBS, header.fieldAndParameterDefaultValueDataOffset, header.fieldAndParameterDefaultValueDataSize, out newHeader.fieldAndParameterDefaultValueDataOffset, out newHeader.fieldAndParameterDefaultValueDataSize);
        bs.CopyMetadataSection(newBS, (int)header.fieldMarshaledSizesOffset, header.fieldMarshaledSizesSize, out newHeader.fieldMarshaledSizesOffset, out newHeader.fieldMarshaledSizesSize);
        bs.CopyMetadataSection(newBS, header.parametersOffset, header.parametersSize, out newHeader.parametersOffset, out newHeader.parametersSize);
        bs.ConvertMetadataSection<MhyIl2Cpp.FieldDefinition, UnityIl2Cpp.FieldDefinition>(newBS, header.fieldsOffset, header.fieldsSize, out newHeader.fieldsOffset, out newHeader.fieldsSize, FieldDefinitionConverter);
        bs.CopyMetadataSection(newBS, header.genericParametersOffset, header.genericParametersSize, out newHeader.genericParametersOffset, out newHeader.genericParametersSize);
        bs.CopyMetadataSection(newBS, header.genericParameterConstraintsOffset, header.genericParameterConstraintsSize, out newHeader.genericParameterConstraintsOffset, out newHeader.genericParameterConstraintsSize);
        bs.CopyMetadataSection(newBS, header.genericContainersOffset, header.genericContainersSize, out newHeader.genericContainersOffset, out newHeader.genericContainersSize);
        bs.CopyMetadataSection(newBS, header.nestedTypesOffset, header.nestedTypesSize, out newHeader.nestedTypesOffset, out newHeader.nestedTypesSize);
        bs.CopyMetadataSection(newBS, header.interfacesOffset, header.interfacesSize, out newHeader.interfacesOffset, out newHeader.interfacesSize);
        bs.CopyMetadataSection(newBS, header.vtableMethodsOffset, header.vtableMethodsSize, out newHeader.vtableMethodsOffset, out newHeader.vtableMethodsSize);
        bs.CopyMetadataSection(newBS, header.interfaceOffsetsOffset, header.interfaceOffsetsSize, out newHeader.interfaceOffsetsOffset, out newHeader.interfaceOffsetsSize);
        bs.ConvertMetadataSection<MhyIl2Cpp.TypeDefinition, UnityIl2Cpp.TypeDefinition>(newBS, header.typeDefinitionsOffset, header.typeDefinitionsSize, out newHeader.typeDefinitionsOffset, out newHeader.typeDefinitionsSize, TypeDefinitionConverter);
        bs.CopyMetadataSection(newBS, header.imagesOffset, header.imagesSize, out newHeader.imagesOffset, out newHeader.imagesSize);
        bs.CopyMetadataSection(newBS, header.assembliesOffset, header.assembliesSize, out newHeader.assembliesOffset, out newHeader.assembliesSize);
        bs.CopyMetadataSection(newBS, header.metadataUsageListsOffset, header.metadataUsageListsCount, out newHeader.metadataUsageListsOffset, out newHeader.metadataUsageListsCount);
        bs.CopyMetadataSection(newBS, header.metadataUsagePairsOffset, header.metadataUsagePairsCount, out newHeader.metadataUsagePairsOffset, out newHeader.metadataUsagePairsCount);
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

    public static UnityIl2Cpp.StringLiteral StringLiteralConverter(MhyIl2Cpp.StringLiteral value)
    {
        return new()
        {
            length = value.length,
            dataIndex = value.dataIndex
        };
    }

    public static UnityIl2Cpp.PropertyDefinition PropertyDefinitionConverter(MhyIl2Cpp.PropertyDefinition value)
    {
        return new()
        {
            nameIndex = value.nameIndex,
            get = value.get,
            set = value.set,
            attrs = value.attrs,
            customAttributeIndex = value.customAttributeIndex,
            token = value.token
        };
    }

    public static UnityIl2Cpp.MethodDefinition MethodDefinitionConverter(MhyIl2Cpp.MethodDefinition value)
    {
        return new()
        {
            nameIndex = value.nameIndex,
            declaringType = value.declaringType,
            returnType = value.returnType,
            parameterStart = value.parameterStart,
            customAttributeIndex = value.customAttributeIndex,
            genericContainerIndex = value.genericContainerIndex,
            methodIndex = value.methodIndex,
            invokerIndex = value.invokerIndex,
            delegateWrapperIndex = value.delegateWrapperIndex,
            rgctxStartIndex = value.rgctxStartIndex,
            rgctxCount = value.rgctxCount,
            token = value.token,
            flags = value.flags,
            iflags = value.iflags,
            slot = value.slot,
            parameterCount = value.parameterCount
        };
    }

    public static UnityIl2Cpp.FieldDefinition FieldDefinitionConverter(MhyIl2Cpp.FieldDefinition value)
    {
        return new()
        {
            nameIndex = value.nameIndex,
            typeIndex = value.typeIndex,
            customAttributeIndex = value.customAttributeIndex,
            token = value.token
        };
    }

    public static UnityIl2Cpp.TypeDefinition TypeDefinitionConverter(MhyIl2Cpp.TypeDefinition value)
    {
        return new()
        {
            nameIndex = value.nameIndex,
            namespaceIndex = value.namespaceIndex,
            customAttributeIndex = value.customAttributeIndex,
            byvalTypeIndex = value.byvalTypeIndex,
            byrefTypeIndex = value.byrefTypeIndex,
            declaringTypeIndex = value.declaringTypeIndex,
            parentIndex = value.parentIndex,
            elementTypeIndex = value.elementTypeIndex,
            rgctxStartIndex = value.rgctxStartIndex,
            rgctxCount = value.rgctxCount,
            genericContainerIndex = value.genericContainerIndex,
            flags = value.flags,
            fieldStart = value.fieldStart,
            methodStart = value.methodStart,
            eventStart = value.eventStart,
            propertyStart = value.propertyStart,
            nestedTypesStart = value.nestedTypesStart,
            interfacesStart = value.interfacesStart,
            vtableStart = value.vtableStart,
            interfaceOffsetsStart = value.interfaceOffsetsStart,
            method_count = value.method_count,
            property_count = value.property_count,
            field_count = value.field_count,
            event_count = value.event_count,
            nested_type_count = value.nested_type_count,
            vtable_count = value.vtable_count,
            interfaces_count = value.interfaces_count,
            interface_offsets_count = value.interface_offsets_count,
            bitfield = value.bitfield,
            token = value.token
        };
    }
}