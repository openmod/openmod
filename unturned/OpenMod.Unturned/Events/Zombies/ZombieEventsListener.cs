using HarmonyLib;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Users;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenMod.Unturned.Events.Zombies
{
    internal class ZombieEventsListener : UnturnedEventsListener
    {
        public ZombieEventsListener(IOpenModHost openModHost,
            IEventBus eventBus,
            IUserManager userManager) : base(openModHost, eventBus, userManager)
        {

        }

        public override void Subscribe()
        {
            OnZombieSpawn += Events_OnZombieSpawn;
        }

        public override void Unsubscribe()
        {
            OnZombieSpawn -= Events_OnZombieSpawn;
        }

        private void Events_OnZombieSpawn(Zombie zombie)
        {
            UnturnedZombieSpawnEvent @event = new UnturnedZombieSpawnEvent(zombie);

            Emit(@event);
        }

        private delegate void ZombieSpawn(Zombie zombie);
        private static event ZombieSpawn OnZombieSpawn;

        [HarmonyPatch]
        private class ZombiePatches
        {
            [HarmonyPatch(typeof(Zombie), "tellAlive")]
            [HarmonyPostfix]
            private static void TellAlive(Zombie __instance)
            {
                OnZombieSpawn?.Invoke(__instance);
            }

            [HarmonyPatch(typeof(ZombieManager), "addZombie")]
            [HarmonyPostfix]
            private static void AddZombie(byte bound)
            {
                Zombie zombie = ZombieManager.regions[bound].zombies.LastOrDefault();

                if (zombie != null)
                {
                    OnZombieSpawn?.Invoke(zombie);
                }
            }
        }
    }
}
