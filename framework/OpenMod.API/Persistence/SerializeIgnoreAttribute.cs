using System;

namespace OpenMod.API.Persistence;

/// <summary>
///     Attribute to mark any property or field to be ignored
///     This make API independent of any type such as json/xml/yaml
/// </summary>
//todo add this to docs
//todo propagate this to all serializers, note this will be implemented when change to VYaml
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter |
                AttributeTargets.ReturnValue)]
public class SerializeIgnoreAttribute : Attribute
{
}