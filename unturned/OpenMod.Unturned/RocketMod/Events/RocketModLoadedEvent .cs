using System.Reflection;
using OpenMod.Core.Eventing;

namespace OpenMod.Unturned.RocketMod.Events
{
    /// <summary>
    /// Fired when the RocketMod assemblies are loaded.
    /// </summary>
    public class RocketModLoadedEvent : Event
    {
        public Assembly RocketModUnturnedAssembly { get; }

        public RocketModLoadedEvent(Assembly rocketModUnturnedAssembly)
        {
            RocketModUnturnedAssembly = rocketModUnturnedAssembly;
        }
    }
}