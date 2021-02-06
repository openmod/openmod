using System;
using System.Collections.Generic;

namespace OpenMod.Core.Commands
{
    [Serializable]
    public sealed class RegisteredCommandsData
    {
        public List<RegisteredCommandData>? Commands { get; set; }
    }
}