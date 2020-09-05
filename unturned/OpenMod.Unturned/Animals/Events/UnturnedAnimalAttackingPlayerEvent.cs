using OpenMod.Unturned.Players;

namespace OpenMod.Unturned.Animals.Events
{
    public class UnturnedAnimalAttackingPlayerEvent : UnturnedAnimalAttackingEvent
    {
        public UnturnedPlayer Player { get; set; }

        public UnturnedAnimalAttackingPlayerEvent(UnturnedAnimal animal, UnturnedPlayer player, bool sendToPack) : base(animal, sendToPack)
        {
            Player = player;
        }
    }
}
