using System;
using System.Threading.Tasks;
using OpenMod.API.Eventing;
using OpenMod.Core.Eventing;
using OpenMod.Unturned.API.Player;
using OpenMod.Unturned.Commands;
using OpenMod.Unturned.World;
using SDG.Unturned;
using UnityEngine;

//TODO: Probably align this to project structure/events and stuff but just putting it here now because nowhere else
namespace OpenMod.Unturned.EventHandling.Player
{
    public class PlayerDeadEventHandling : IEventListener
    {

        [EventListener]
        public async Task Handle(Vector3 location, UnturnedPlayerCommandActor player, EDeathCause cause)
        {
            player.LastDeath = new PlayerDeath()
            {
                PlayerDeathLocation = new PlayerDeathLocation(){Location = location, Player = player},
                Player = player,
                Time = DateTime.Now.TimeOfDay,
                Cause = cause
            };
        }
        
    }
}