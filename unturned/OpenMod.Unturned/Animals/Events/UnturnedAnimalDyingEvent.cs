using SDG.Unturned;
using System.Numerics;
using Steamworks;

namespace OpenMod.Unturned.Animals.Events
{
    public class UnturnedAnimalDyingEvent : UnturnedAnimalDamagingEvent
    {
        public UnturnedAnimalDyingEvent(UnturnedAnimal animal, ushort damageAmount, Vector3 ragdoll,
            ERagdollEffect ragdollEffect, CSteamID instigator) : base(animal, damageAmount, ragdoll,
            ragdollEffect, instigator)
        {
        }
    }
}