using System;
using System.Collections.Generic;

using JetBrains.Annotations;

using OpenMod.API;
using OpenMod.Unturned.Events;
using OpenMod.Unturned.Players;

using SDG.Unturned;

using Steamworks;

using UnityEngine;

namespace OpenMod.Unturned.Resources.Events
{
    [OpenModInternal]
    [UsedImplicitly]
    internal class ResourcesEventsListener : UnturnedEventsListener
    {
        public ResourcesEventsListener(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override void Subscribe() {
            ResourceManager.onDamageResourceRequested += OnDamageResourceRequested;
        }

        public override void Unsubscribe()
        {
            ResourceManager.onDamageResourceRequested -= OnDamageResourceRequested;
        }

        private void OnDamageResourceRequested(CSteamID instigatorSteamId, Transform objectTransform, // lgtm [cs/too-many-ref-parameters]
            ref ushort pendingTotalDamage, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            if (!ResourceManager.tryGetRegion(objectTransform, out byte x, out byte y, out ushort index))
            {
                return;
            }

            List<ResourceSpawnpoint> tree = LevelGround.trees[x, y];
            ResourceSpawnpoint spawnpoint = tree[index];

            Player? nativePlayer = PlayerTool.getPlayer(instigatorSteamId);
            UnturnedPlayer? player = GetUnturnedPlayer(nativePlayer);

            var @event = new UnturnedResourceDamagingEvent(spawnpoint, pendingTotalDamage, damageOrigin, player, instigatorSteamId)
            {
                IsCancelled = !shouldAllow
            };

            Emit(@event);

            pendingTotalDamage = @event.DamageAmount;
            shouldAllow = !@event.IsCancelled;
        }
    }
}