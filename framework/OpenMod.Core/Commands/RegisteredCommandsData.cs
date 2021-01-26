using System;
using System.Collections.Generic;

namespace OpenMod.Core.Commands
{
    [Serializable]
    public class RegisteredCommandsData
    {
        public List<RegisteredCommandData>? Commands { get; set; }
    }
}