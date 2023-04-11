using OpenMod.API;
using OpenMod.Unturned.Events;
using System;

namespace OpenMod.Unturned.Level.Events
{
    [OpenModInternal]
    internal class LevelEventsListener : UnturnedEventsListener
    {
        public LevelEventsListener(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        public override void Subscribe()
        {
            SDG.Unturned.Level.onLevelsRefreshed += OnLevelsRefreshed;
            SDG.Unturned.Level.onPostLevelLoaded += OnPostLevelLoaded;
            SDG.Unturned.Level.onPreLevelLoaded += OnPreLevelLoaded;
            SDG.Unturned.Level.onPrePreLevelLoaded += OnPrePreLevelLoaded;
            SDG.Unturned.Level.onLevelLoaded += OnLevelLoaded;
        }

        public override void Unsubscribe()
        {
            SDG.Unturned.Level.onLevelsRefreshed -= OnLevelsRefreshed;
            SDG.Unturned.Level.onPostLevelLoaded -= OnPostLevelLoaded;
            SDG.Unturned.Level.onPreLevelLoaded -= OnPreLevelLoaded;
            SDG.Unturned.Level.onPrePreLevelLoaded -= OnPrePreLevelLoaded;
            SDG.Unturned.Level.onLevelLoaded -= OnLevelLoaded;
        }

        private void OnLevelsRefreshed()
        {
            var @event = new UnturnedLevelsRefreshedEvent();
            Emit(@event);
        }

        private void OnPostLevelLoaded(int level)
        {
            var @event = new UnturnedLevelEvent(level);
            Emit(@event);
        }

        private void OnPreLevelLoaded(int level)
        {
            var @event = new UnturnedLevelEvent(level);
            Emit(@event);
        }

        private void OnPrePreLevelLoaded(int level)
        {
            var @event = new UnturnedLevelEvent(level);
            Emit(@event);
        }

        private void OnLevelLoaded(int level)
        {
            var @event = new UnturnedLevelEvent(level);
            Emit(@event);
        }
    }
}
