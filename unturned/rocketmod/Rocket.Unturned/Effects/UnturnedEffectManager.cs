using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;

namespace Rocket.Unturned.Effects
{
    public class UnturnedEffect
    {
        public UnturnedEffect(string type, ushort effectID, bool global)
        {
            this.Type = type;
            this.EffectID = effectID;
            this.Global = global;
        }
        public string Type;
        public ushort EffectID;
        public bool Global;

        public void Trigger(UnturnedPlayer player)
        {
            if (!Global)
            {
                SDG.Unturned.EffectManager.instance.channel.send("tellEffectPoint", player.CSteamID, ESteamPacket.UPDATE_UNRELIABLE_BUFFER, new object[] { EffectID, player.Player.transform.position });
            }
            else
            {
                SDG.Unturned.EffectManager.instance.channel.send("tellEffectPoint", ESteamCall.CLIENTS, player.Player.transform.position, 1024, ESteamPacket.UPDATE_UNRELIABLE_BUFFER, new object[] { EffectID, player.Player.transform.position });
            }
        }

        public void Trigger(Vector3 position)
        {
            SDG.Unturned.EffectManager.instance.channel.send("tellEffectPoint", ESteamCall.CLIENTS, position, 1024, ESteamPacket.UPDATE_UNRELIABLE_BUFFER, new object[] { EffectID, position });
        }
    }
}
