extern alias JetBrainsAnnotations;
using System;
using JetBrainsAnnotations::JetBrains.Annotations;
using OpenMod.Unturned.Events;
using SDG.Unturned;

// ReSharper disable InconsistentNaming

namespace OpenMod.Unturned.Players.Skills.Events
{
    [UsedImplicitly]
    internal class PlayerSkillsEventsListener : UnturnedEventsListener
    {
        public PlayerSkillsEventsListener(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override void Subscribe()
        {
            PlayerSkills.OnExperienceChanged_Global += PlayerSkillsOnOnExperienceChanged_Global;
        }

        public override void Unsubscribe()
        {
            PlayerSkills.OnExperienceChanged_Global -= PlayerSkillsOnOnExperienceChanged_Global;
        }

        private void PlayerSkillsOnOnExperienceChanged_Global(PlayerSkills skills, uint experience)
        {
            var player = GetUnturnedPlayer(skills.player)!;
            var @event = new UnturnedPlayerExperienceUpdatedEvent(player, experience);

            Emit(@event);
        }
    }
}