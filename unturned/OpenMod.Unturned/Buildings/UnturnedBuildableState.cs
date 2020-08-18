using OpenMod.Extensions.Games.Abstractions.Buildings;
using SDG.Unturned;

namespace OpenMod.Unturned.Buildings
{
    public class UnturnedBuildableState : IBuildableState
    {
        public ushort Health { get; }

        public byte[] State { get; }

        public UnturnedBuildableState(Barricade barricade)
        {
            Health = barricade.health;
            State = barricade.state;
        }

        public UnturnedBuildableState(Structure structure)
        {
            Health = structure.health;
        }
    }
}