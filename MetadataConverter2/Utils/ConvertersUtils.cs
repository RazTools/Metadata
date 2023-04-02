using MetadataConverter2.IL2CPP;
using static MetadataConverter2.IL2CPP.UnityIl2Cpp;

namespace MetadataConverter2.Utils
{
    public static class ConvertersUtils
    {
        public static StringLiteral StringLiteralConverter(MhyIl2Cpp.StringLiteral value)
        {
            return new()
            {
                length = value.length,
                dataIndex = value.dataIndex
            };
        }

        public static PropertyDefinition PropertyDefinitionConverter(MhyIl2Cpp.PropertyDefinition value)
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

        public static MethodDefinition MethodDefinitionConverter(MhyIl2Cpp.MethodDefinition value)
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

        public static FieldDefinition FieldDefinitionConverter(MhyIl2Cpp.FieldDefinition value)
        {
            return new()
            {
                nameIndex = value.nameIndex,
                typeIndex = value.typeIndex,
                customAttributeIndex = value.customAttributeIndex,
                token = value.token
            };
        }

        public static TypeDefinition TypeDefinitionConverter(MhyIl2Cpp.TypeDefinition value)
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
}
