using System;

using JetBrains.Annotations;

using OpenMod.API;
using OpenMod.Unturned.Events;

using SDG.Unturned;

namespace OpenMod.Unturned.Workshop.Events
{
    [OpenModInternal]
    [UsedImplicitly]
    internal class WorkshopEventsListener : UnturnedEventsListener
    {

        public WorkshopEventsListener(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override void Subscribe()
        {
            DedicatedUGC.installed += WorkshopLoadedEventEmitter;
        }

        public override void Unsubscribe()
        {
            DedicatedUGC.installed -= WorkshopLoadedEventEmitter;
        }

        private void WorkshopLoadedEventEmitter()
        {
            Emit(new UnturnedWorkshopLoadedEvent());
        }

    }
}