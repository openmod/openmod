using UnityEngine;

namespace OpenMod.Unturned.Zombies.Events
{
    public class UnturnedZombieAlertingPositionEvent : UnturnedZombieAlertingEvent
    {
        public Vector3 Position { get; set; }

        public bool IsStartling { get; set; }

        public UnturnedZombieAlertingPositionEvent(UnturnedZombie zombie, Vector3 position, bool isStartling) : base(zombie)
        {
            Position = position;
            IsStartling = isStartling;
        }
    }
}
