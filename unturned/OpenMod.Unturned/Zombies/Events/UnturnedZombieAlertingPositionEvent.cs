using UnityEngine;

namespace OpenMod.Unturned.Zombies.Events
{
    /// <summary>
    /// The event that is triggered when a zombie has been alerted to a position.
    /// </summary>
    public class UnturnedZombieAlertingPositionEvent : UnturnedZombieAlertingEvent
    {
        /// <summary>
        /// The alert position.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// True if startling.
        /// </summary>
        public bool IsStartling { get; set; }

        public UnturnedZombieAlertingPositionEvent(UnturnedZombie zombie, Vector3 position, bool isStartling) : base(zombie)
        {
            Position = position;
            IsStartling = isStartling;
        }
    }
}
