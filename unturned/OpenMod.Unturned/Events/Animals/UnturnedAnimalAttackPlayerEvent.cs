using OpenMod.Unturned.Entities;

namespace OpenMod.Unturned.Events.Animals
{
    public class UnturnedAnimalAttackPlayerEvent : UnturnedAnimalAttackEvent
    {
        public UnturnedPlayer Player { get; set; }

        public UnturnedAnimalAttackPlayerEvent(UnturnedAnimal animal, UnturnedPlayer player, bool sendToPack) : base(animal, sendToPack)
        {
            Player = player;
        }
    }
}
