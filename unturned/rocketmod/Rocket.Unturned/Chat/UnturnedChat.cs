using Rocket.Core;
using Rocket.Core.Logging;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Rocket.API;

namespace Rocket.Unturned.Chat
{
    public sealed class UnturnedChat : MonoBehaviour
    {
        private void Awake()
        {
            SDG.Unturned.ChatManager.onChatted += handleChat;
        }

        private void handleChat(SteamPlayer steamPlayer, EChatMode chatMode, ref Color incomingColor, ref bool rich, string message, ref bool cancel)
        {
            cancel = false;
            Color color = incomingColor;
            try
            {
                UnturnedPlayer player = UnturnedPlayer.FromSteamPlayer(steamPlayer);
                color = UnturnedPlayerEvents.firePlayerChatted(player, chatMode, player.Color, message, ref cancel);
            }
            catch (Exception ex)
            {
                Core.Logging.Logger.LogException(ex);
            }

            cancel = !cancel;
            incomingColor = color;
        }

        public static Color GetColorFromName(string colorName, Color fallback)
        {
            switch (colorName.Trim().ToLower())
            {
                case "black": return Color.black;
                case "blue": return Color.blue;
                case "clear": return Color.clear;
                case "cyan": return Color.cyan;
                case "gray": return Color.gray;
                case "green": return Color.green;
                case "grey": return Color.grey;
                case "magenta": return Color.magenta;
                case "red": return Color.red;
                case "white": return Color.white;
                case "yellow": return Color.yellow;
                case "rocket": return GetColorFromRGB(90, 206, 205);
            }

            Color? color = GetColorFromHex(colorName);
            if (color.HasValue) return color.Value;

            return fallback;
        }

        public static Color? GetColorFromHex(string hexString)
        {
            hexString = hexString.Replace("#", "");
            if(hexString.Length == 3)
            { // #99f
                hexString = hexString.Insert(1, System.Convert.ToString(hexString[0])); // #999f
                hexString = hexString.Insert(3, System.Convert.ToString(hexString[2])); // #9999f
                hexString = hexString.Insert(5, System.Convert.ToString(hexString[4])); // #9999ff
            }
            int argb;
            if (hexString.Length != 6 || !Int32.TryParse(hexString, System.Globalization.NumberStyles.HexNumber, null, out argb))
            {
                return null;
            }
            byte r = (byte)((argb >> 16) & 0xff);
            byte g = (byte)((argb >> 8) & 0xff);
            byte b = (byte)(argb & 0xff);
            return GetColorFromRGB(r, g, b);
        }
		public static Color GetColorFromRGB(byte R,byte G,byte B)
		{
			return GetColorFromRGB (R, G, B, 100);
		}
        public static Color GetColorFromRGB(byte R,byte G,byte B,short A)
        {
            return new Color((1f / 255f) * R, (1f / 255f) * G, (1f / 255f) * B,(1f/100f) * A);
        }

        public static void Say(string message, bool rich)
        {
            Say(message, Palette.SERVER, rich);
        }

        public static void Say(string message)
        {
            Say(message, false);
        }

        public static void Say(IRocketPlayer player, string message)
        {
            Say(player, message, false);
        }

        public static void Say(IRocketPlayer player, string message, Color color, bool rich)
        {
            if (player is ConsolePlayer)
            {
                Core.Logging.Logger.Log(message, ConsoleColor.Gray);
            }
            else
            {
                Say(new CSteamID(ulong.Parse(player.Id)), message, color, rich);
            }
        }

        public static void Say(IRocketPlayer player, string message, Color color)
        {
            Say(player, message, color, false);
        }

        public static void Say(string message, Color color, bool rich)
        {
            Core.Logging.Logger.Log("Broadcast: " + message, ConsoleColor.Gray);
            foreach(string m in wrapMessage(message))
            {
                ChatManager.serverSendMessage(m, color, fromPlayer: null, toPlayer: null, mode: EChatMode.GLOBAL, iconURL: null, useRichTextFormatting: rich);
            }
        }
        
        public static void Say(string message, Color color)
        {
            Say(message, color, false);
        }
        
        public static void Say(IRocketPlayer player, string message, bool rich)
        {
            Say(player, message, Palette.SERVER, rich);
        }

        public static void Say(CSteamID CSteamID, string message, bool rich)
        {
            Say(CSteamID, message, Palette.SERVER, rich);
        }


        public static void Say(CSteamID CSteamID, string message)
        {
            Say(CSteamID, message, false);
        }

        public static void Say(CSteamID CSteamID, string message, Color color, bool rich)
        {
            if (CSteamID == null || CSteamID.ToString() == "0")
            {
                Core.Logging.Logger.Log(message, ConsoleColor.Gray);
            }
            else
            {
                SteamPlayer toPlayer = PlayerTool.getSteamPlayer(CSteamID);
                foreach(string m in wrapMessage(message))
                {
                    ChatManager.serverSendMessage(m, color, fromPlayer: null, toPlayer: toPlayer, mode: EChatMode.SAY, iconURL: null, useRichTextFormatting: rich);
                }
            }
        }

        public static void Say(CSteamID CSteamID, string message, Color color)
        {
            Say(CSteamID, message, color, false);
        }

         public static List<string> wrapMessage(string text)
         {
             if (text.Length == 0) return new List<string>();
             string[] words = text.Split(' ');
             List<string> lines = new List<string>();
             string currentLine = "";
             int maxLength = 90;
             foreach (var currentWord in words)
             {
  
                 if ((currentLine.Length > maxLength) ||
                     ((currentLine.Length + currentWord.Length) > maxLength))
                 {
                     lines.Add(currentLine);
                     currentLine = "";
                 }
  
                 if (currentLine.Length > 0)
                     currentLine += " " + currentWord;
                 else
                     currentLine += currentWord;
  
             }
  
             if (currentLine.Length > 0)
                 lines.Add(currentLine);
                 return lines;
            }
    }
}