using System;
using OpenMod.API;
using OpenMod.Unturned.Events;
using SDG.Unturned;

namespace OpenMod.Unturned.Persistence.Events
{
    [OpenModInternal]
    internal class PersistenceEventsListener : UnturnedEventsListener
    {
        public PersistenceEventsListener(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override void Subscribe()
        {
            SaveManager.onPostSave += OnPostSave;
        }

        public override void Unsubscribe()
        {
            SaveManager.onPostSave -= OnPostSave;
        }

        private void OnPostSave()
        {
            var @event = new UnturnedGameSavedEvent();

            Emit(@event);
        }
    }
}