using OpenMod.Extensions.Games.Abstractions.Building;
using SDG.Unturned;

namespace OpenMod.Unturned.Building
{
    public class UnturnedBuildableState : IBuildableState
    {
        public double Health { get; }

        public byte[]? StateData { get; }

        public UnturnedBuildableState(Barricade barricade)
        {
            Health = barricade.health;
            StateData = barricade.state;
        }

        public UnturnedBuildableState(Structure structure)
        {
            Health = structure.health;
        }
    }
}