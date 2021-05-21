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

            LeanType leanType;

            switch (obj.lean)
            {
                case 1:
                    leanType = LeanType.Left;
                    break;
                case 0:
                    leanType = LeanType.Center;
                    break;
                case -1:
                    leanType = LeanType.Right;
                    break;
                default:
                    leanType = LeanType.Center;
                    break;
            }

            var @event = new UnturnedPlayerLeanUpdatedEvent(player, leanType);

            Emit(@event);
        }

        public override void Unsubscribe()
        {
            PlayerAnimator.OnLeanChanged_Global -= OnLeanChanged;
        }
    }
}