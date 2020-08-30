using SDG.Unturned;
using UnityEngine;

namespace OpenMod.Unturned.Events.Zombies
{
    public class UnturnedZombieAlertPositionEvent : UnturnedZombieAlertEvent
    {
        public Vector3 Position { get; set; }

        public bool IsStartling { get; set; }

        public UnturnedZombieAlertPositionEvent(Zombie zombie, Vector3 position, bool isStartling) : base(zombie)
        {
            Position = position;
            IsStartling = isStartling;
        }
    }
}
