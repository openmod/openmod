using System;
using OpenMod.Unturned.Events;
using SDG.Unturned;

namespace OpenMod.Unturned.Players.Animator.Events
{
    internal class AnimatorEventsListener : UnturnedEventsListener
    {
        public AnimatorEventsListener(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override void Subscribe()
        {
            PlayerAnimator.OnLeanChanged_Global += OnLeanChanged;
        }

        private void OnLeanChanged(PlayerAnimator obj)
        {
            var player = GetUnturnedPlayer(obj.player)!;
            var leanType = obj.lean switch
            {
                1 => LeanType.Left,
                0 => LeanType.Center,
                -1 => LeanType.Right,
                _ => LeanType.Center,
            };
            var @event = new UnturnedPlayerLeanUpdatedEvent(player, leanType);

            Emit(@event);
        }

        public override void Unsubscribe()
        {
            PlayerAnimator.OnLeanChanged_Global -= OnLeanChanged;
        }
    }
}