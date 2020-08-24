using OpenMod.Core.Eventing;
using SDG.Unturned;

namespace OpenMod.Unturned.Events.Environment
{
    public class UnturnedSnowUpdateEvent : Event
    {
        public ELightingSnow Snow { get; }

        public UnturnedSnowUpdateEvent(ELightingSnow snow)
        {
            Snow = snow;
        }
    }
}
