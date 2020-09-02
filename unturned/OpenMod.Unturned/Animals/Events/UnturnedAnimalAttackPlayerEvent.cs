using OpenMod.Unturned.Players;

namespace OpenMod.Unturned.Animals.Events
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
