using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System;
using System.Linq;
using UnityEngine;

namespace Rocket.Unturned.Commands
{
    public static class UnturnedCommandExtensions
    {
        public static UnturnedPlayer GetUnturnedPlayerParameter(this string[] array, int index)
        {
            return (array.Length <= index) ? null : UnturnedPlayer.FromName(array[index]);
        }

        public static RocketPlayer GetRocketPlayerParameter(this string[] array, int index)
        {
            if(array.Length > index)
            {
                ulong id = 0;
                if (ulong.TryParse(array[index], out id) && id > 76561197960265728)
                {
                    return new RocketPlayer(id.ToString());
                }
            }
            return null;
        }

        public static ulong? GetCSteamIDParameter(this string[] array, int index)
        {
            if (array.Length > index)
            {
                ulong id = 0;
                if (ulong.TryParse(array[index], out id) && id > 76561197960265728)
                {
                    return id;
                }
            }
            return null;
        }

        public static Color? GetColorParameter(this string[] array, int index)
        {
            if(array.Length <= index) return null;
            Color output = UnturnedChat.GetColorFromName(array[index], Color.clear);
            return (output == Color.clear) ? null : (Color?)output;
        }
    }
}
