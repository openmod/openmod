using System;
using System.Collections.Generic;
using VYaml.Annotations;

namespace OpenMod.Core.Commands
{
    [Serializable, YamlObject]
    public sealed partial class RegisteredCommandsData
    {
        public List<RegisteredCommandData>? Commands { get; set; }
    }
}