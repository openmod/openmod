using OpenMod.API.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.TypeInspectors;

namespace OpenMod.Core.Persistence.Yaml
{
    [Obsolete("Moving from YamlDotNet to VYaml, this is kept for compatibility reasons")]
    public class SerializableIgnoreInspector(ITypeInspector typeInspector) : TypeInspectorSkeleton
    {
        public override string GetEnumName(Type enumType, string name)
        {
            return typeInspector.GetEnumName(enumType, name);
        }

        public override string GetEnumValue(object enumValue)
        {
            return typeInspector.GetEnumValue(enumValue);
        }

        public override IEnumerable<IPropertyDescriptor> GetProperties(Type type, object? container)
        {
            var inspectorProps = typeInspector.GetProperties(type, container);
            return inspectorProps.Where(inspectProp => inspectProp.GetCustomAttribute<SerializeIgnoreAttribute>() == null);
        }
    }
}
