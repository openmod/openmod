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
            SDG.Unturned.Level.onPostLevelLoaded += OnPostLevelLoaded;
            SDG.Unturned.Level.onPreLevelLoaded += OnPreLevelLoaded;
            SDG.Unturned.Level.onPrePreLevelLoaded += OnPrePreLevelLoaded;
            SDG.Unturned.Level.onLevelLoaded += OnLevelLoaded;
        }

        public override void Unsubscribe()
        {
            SDG.Unturned.Level.onPostLevelLoaded -= OnPostLevelLoaded;
            SDG.Unturned.Level.onPreLevelLoaded -= OnPreLevelLoaded;
            SDG.Unturned.Level.onPrePreLevelLoaded -= OnPrePreLevelLoaded;
            SDG.Unturned.Level.onLevelLoaded -= OnLevelLoaded;
        }

        private void OnPostLevelLoaded(int level)
        {
            var @event = new UnturnedPostLevelLoadedEvent();
            Emit(@event);
        }

        private void OnPreLevelLoaded(int level)
        {
            if (level != SDG.Unturned.Level.BUILD_INDEX_GAME) return;
            var @event = new UnturnedPreLevelLoadedEvent();
            Emit(@event);
        }

        private void OnPrePreLevelLoaded(int level)
        {
            if (level != SDG.Unturned.Level.BUILD_INDEX_GAME) return;
            var @event = new UnturnedPrePreLevelLoadedEvent();
            Emit(@event);
        }

        private void OnLevelLoaded(int level)
        {
            if (level != SDG.Unturned.Level.BUILD_INDEX_GAME) return;
            var @event = new UnturnedLevelLoadedEvent();
            Emit(@event);
        }
    }
}
