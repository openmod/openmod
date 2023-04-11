using OpenMod.Core.Eventing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenMod.Unturned.Level.Events
{
    /// <summary>
    /// The event that is triggered when level has been loaded.
    /// </summary>
    public class UnturnedLevelLoadedEvent : Event
    {
        public int Level { get; set; }

        public UnturnedLevelLoadedEvent(int level)
        {
            Level = level;
        }
    }
}
