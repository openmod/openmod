using Rocket.Core.Logging;
using Rocket.Unturned.Enumerations;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using UnityEngine;
using System.Linq;
using Rocket.Core.Extensions;

namespace Rocket.Unturned.Events
{
    public sealed class UnturnedPlayerEvents : UnturnedPlayerComponent
    {
        protected override void Load()
        {
            Player.Player.life.onStaminaUpdated += onUpdateStamina;
            Player.Player.inventory.onInventoryAdded += onInventoryAdded;
            Player.Player.inventory.onInventoryRemoved += onInventoryRemoved;
            Player.Player.inventory.onInventoryResized += onInventoryResized;
            Player.Player.inventory.onInventoryUpdated += onInventoryUpdated;
        }

        // OPENMOD PATCH: Unregister callbacks on destroy
        protected override void Unload()
        {
            Player.Player.life.onStaminaUpdated -= onUpdateStamina;
            Player.Player.inventory.onInventoryAdded -= onInventoryAdded;
            Player.Player.inventory.onInventoryRemoved -= onInventoryRemoved;
            Player.Player.inventory.onInventoryResized -= onInventoryResized;
            Player.Player.inventory.onInventoryUpdated -= onInventoryUpdated;
        }
        // END OPENMOD PATCH: Unregister callbacks on destroy
        
        private void Start()
        {
            UnturnedEvents.triggerOnPlayerConnected(Player);
        }

        internal static void TriggerReceive(SteamChannel instance, CSteamID d, byte[] a, int b,int size)
        {
#if DEBUG
            /*ESteamPacket eSteamPacket = (ESteamPacket)a[0];
            int num = a[1];

            if (eSteamPacket != ESteamPacket.UPDATE_VOICE && eSteamPacket != ESteamPacket.UPDATE_UDP_CHUNK && eSteamPacket != ESteamPacket.UPDATE_TCP_CHUNK)
            {
                object[] objects = SteamPacker.getObjects(d, 2, a, instance.Methods[num].Types);

                string o = "";
                foreach (object r in objects)
                {
                    o += r.ToString() + ",";
                }
                Logger.Log("Receive+" + d.ToString() + ": " + o + " - " + b);
            }*/
#endif
            return;
        }
        
        internal static void TriggerSend(SteamPlayer s, string W, ESteamCall X, ESteamPacket l, params object[] R)
        {
            try
            {
                if (s == null || s.player == null || s.playerID.steamID == CSteamID.Nil || s.player.transform == null || R == null) return;
                UnturnedPlayerEvents instance = s.player.transform.GetComponent<UnturnedPlayerEvents>();
                UnturnedPlayer rp = UnturnedPlayer.FromSteamPlayer(s);
#if DEBUG
                 //string o = "";
                 //foreach (object r in R)
                 //{
                 //    o += r.ToString();
                 //}
                 //Logger.Log("Send+" + s.SteamPlayerID.CSteamID.ToString() + ": " + W + " - " + o);
#endif
                if (W.StartsWith("tellWear")) {
                    OnPlayerWear.TryInvoke(rp, Enum.Parse(typeof(Wearables), W.Replace("tellWear", "")), (ushort)R[0], R.Count() > 1 ? (byte?)R[1] : null);
                }
                switch (W)
                {
                    case "tellBleeding":
                        OnPlayerUpdateBleeding.TryInvoke(rp, (bool)R[0]);
                        instance.OnUpdateBleeding.TryInvoke( rp, (bool)R[0]);
                        break;
                    case "tellBroken":
                        OnPlayerUpdateBroken.TryInvoke(rp, (bool)R[0]);
                        instance.OnUpdateBroken.TryInvoke(rp, (bool)R[0]);
                        break;
                    case "tellLife":
                        OnPlayerUpdateLife.TryInvoke(rp, (byte)R[0]);
                        instance.OnUpdateLife.TryInvoke(rp, (byte)R[0]);
                        break;
                    case "tellFood":
                        OnPlayerUpdateFood.TryInvoke(rp, (byte)R[0]);
                        instance.OnUpdateFood.TryInvoke(rp, (byte)R[0]);
                        break;
                    case "tellHealth":
                        OnPlayerUpdateHealth.TryInvoke(rp, (byte)R[0]);
                        instance.OnUpdateHealth.TryInvoke(rp, (byte)R[0]);
                        break;
                    case "tellVirus":
                        OnPlayerUpdateVirus.TryInvoke(rp, (byte)R[0]);
                        instance.OnUpdateVirus.TryInvoke(rp, (byte)R[0]);
                        break;
                    case "tellWater":
                        OnPlayerUpdateWater.TryInvoke(rp, (byte)R[0]);
                        instance.OnUpdateWater.TryInvoke(rp, (byte)R[0]);
                        break;
                    case "tellStance":
                        OnPlayerUpdateStance.TryInvoke(rp, (byte)R[0]);
                        instance.OnUpdateStance.TryInvoke( rp, (byte)R[0]);
                        break;
                    case "tellGesture":
                        OnPlayerUpdateGesture.TryInvoke(rp, (PlayerGesture)Enum.Parse(typeof(PlayerGesture), R[0].ToString()));
                        instance.OnUpdateGesture.TryInvoke( rp, (PlayerGesture)Enum.Parse(typeof(PlayerGesture), R[0].ToString()));
                        break;
                    case "tellStat":
                        OnPlayerUpdateStat.TryInvoke(rp, (EPlayerStat)(byte)R[0]);
                        instance.OnUpdateStat.TryInvoke(rp, (EPlayerStat)(byte)R[0]);
                        break;
                    case "tellExperience":
                        OnPlayerUpdateExperience.TryInvoke(rp, (uint)R[0]);
                        instance.OnUpdateExperience.TryInvoke(rp, (uint)R[0]);
                        break;
                    case "tellRevive":
                        OnPlayerRevive.TryInvoke(rp, (Vector3)R[0], (byte)R[1]);
                        instance.OnRevive.TryInvoke(rp, (Vector3)R[0], (byte)R[1]);
                        break;
                    case "tellDead":
                        OnPlayerDead.TryInvoke(rp, (Vector3)R[0]);
                        instance.OnDead.TryInvoke(rp, (Vector3)R[0]);
                        break;
                    case "tellDeath":
                        OnPlayerDeath.TryInvoke(rp, (EDeathCause)(byte)R[0], (ELimb)(byte)R[1], new CSteamID(ulong.Parse(R[2].ToString())));
                        instance.OnDeath.TryInvoke(rp, (EDeathCause)(byte)R[0], (ELimb)(byte)R[1], new CSteamID(ulong.Parse(R[2].ToString())));
                        break;
                    default:
#if DEBUG
                       // Logger.Log("Send+" + s.SteamPlayerID.CSteamID.ToString() + ": " + W + " - " + String.Join(",",R.Select(e => e.ToString()).ToArray()));
#endif
                        break;
                }
                return;
            }
            catch (Exception ex)
            {
                Core.Logging.Logger.LogException(ex,"Failed to receive packet \""+W+"\"");
            }
        }


        public delegate void PlayerUpdatePosition(UnturnedPlayer player, Vector3 position);
        public static event PlayerUpdatePosition OnPlayerUpdatePosition;
        internal static void fireOnPlayerUpdatePosition(UnturnedPlayer player)
        {
            OnPlayerUpdatePosition.TryInvoke(player,player.Position);
        }

        public delegate void PlayerUpdateBleeding(UnturnedPlayer player, bool bleeding);
        public static event PlayerUpdateBleeding OnPlayerUpdateBleeding;
        public event PlayerUpdateBleeding OnUpdateBleeding;

        public delegate void PlayerUpdateBroken(UnturnedPlayer player, bool broken);
        public static event PlayerUpdateBroken OnPlayerUpdateBroken;
        public event PlayerUpdateBroken OnUpdateBroken;

        public delegate void PlayerDeath(UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID murderer);
        public static event PlayerDeath OnPlayerDeath;
        public event PlayerDeath OnDeath;

        public delegate void PlayerDead(UnturnedPlayer player, Vector3 position);
        public static event PlayerDead OnPlayerDead;
        public event PlayerDead OnDead;

        public delegate void PlayerUpdateLife(UnturnedPlayer player, byte life);
        public static event PlayerUpdateLife OnPlayerUpdateLife;
        public event PlayerUpdateLife OnUpdateLife;

        public delegate void PlayerUpdateFood(UnturnedPlayer player, byte food);
        public static event PlayerUpdateFood OnPlayerUpdateFood;
        public event PlayerUpdateFood OnUpdateFood;

        public delegate void PlayerUpdateHealth(UnturnedPlayer player, byte health);
        public static event PlayerUpdateHealth OnPlayerUpdateHealth;
        public event PlayerUpdateHealth OnUpdateHealth;

        public delegate void PlayerUpdateVirus(UnturnedPlayer player, byte virus);
        public static event PlayerUpdateVirus OnPlayerUpdateVirus;
        public event PlayerUpdateVirus OnUpdateVirus;

        public delegate void PlayerUpdateWater(UnturnedPlayer player, byte water);
        public static event PlayerUpdateWater OnPlayerUpdateWater;
        public event PlayerUpdateWater OnUpdateWater;

        public enum PlayerGesture { None = 0, InventoryOpen = 1, InventoryClose = 2, Pickup = 3, PunchLeft = 4, PunchRight = 5, SurrenderStart = 6, SurrenderStop = 7, Point = 8, Wave = 9 , Salute = 10 , Arrest_Start = 11 , Arrest_Stop = 12 , Rest_Start = 13 , Rest_Stop = 14 , Facepalm = 15 };
        public delegate void PlayerUpdateGesture(UnturnedPlayer player, PlayerGesture gesture);
        public static event PlayerUpdateGesture OnPlayerUpdateGesture;
        public event PlayerUpdateGesture OnUpdateGesture;

        public delegate void PlayerUpdateStance(UnturnedPlayer player, byte stance);
        public static event PlayerUpdateStance OnPlayerUpdateStance;
        public event PlayerUpdateStance OnUpdateStance;

        public delegate void PlayerRevive(UnturnedPlayer player, Vector3 position, byte angle);
        public static event PlayerRevive OnPlayerRevive;
        public event PlayerRevive OnRevive;

        public delegate void PlayerUpdateStat(UnturnedPlayer player, EPlayerStat stat);
        public static event PlayerUpdateStat OnPlayerUpdateStat;
        public event PlayerUpdateStat OnUpdateStat;

        public delegate void PlayerUpdateExperience(UnturnedPlayer player, uint experience);
        public static event PlayerUpdateExperience OnPlayerUpdateExperience;
        public event PlayerUpdateExperience OnUpdateExperience;

        public delegate void PlayerUpdateStamina(UnturnedPlayer player, byte stamina);
        public static event PlayerUpdateStamina OnPlayerUpdateStamina;
        public event PlayerUpdateStamina OnUpdateStamina;

        private void onUpdateStamina(byte stamina)
        {
            OnPlayerUpdateStamina.TryInvoke(Player, stamina);
            OnUpdateStamina.TryInvoke(Player, stamina);
        }

        public delegate void PlayerInventoryUpdated(UnturnedPlayer player, InventoryGroup inventoryGroup, byte inventoryIndex, ItemJar P);
        public static event PlayerInventoryUpdated OnPlayerInventoryUpdated;
        public event PlayerInventoryUpdated OnInventoryUpdated;

        private void onInventoryUpdated(byte E, byte O, ItemJar P)
        {
            OnPlayerInventoryUpdated.TryInvoke(Player, (InventoryGroup)Enum.Parse(typeof(InventoryGroup), E.ToString()), O, P);
            OnInventoryUpdated.TryInvoke(Player, (InventoryGroup)Enum.Parse(typeof(InventoryGroup), E.ToString()), O, P);
        }

        public delegate void PlayerInventoryResized(UnturnedPlayer player, InventoryGroup inventoryGroup, byte O, byte U);
        public static event PlayerInventoryResized OnPlayerInventoryResized;
        public event PlayerInventoryResized OnInventoryResized;

        private void onInventoryResized(byte E, byte M, byte U)
        {
            OnPlayerInventoryResized.TryInvoke(Player, (InventoryGroup)Enum.Parse(typeof(InventoryGroup), E.ToString()), M, U);
            OnInventoryResized.TryInvoke(Player, (InventoryGroup)Enum.Parse(typeof(InventoryGroup), E.ToString()), M, U);
        }

        public delegate void PlayerInventoryRemoved(UnturnedPlayer player, InventoryGroup inventoryGroup, byte inventoryIndex, ItemJar P);
        public static event PlayerInventoryRemoved OnPlayerInventoryRemoved;
        public event PlayerInventoryRemoved OnInventoryRemoved;

        private void onInventoryRemoved(byte E, byte y, ItemJar f)
        {
            OnPlayerInventoryRemoved.TryInvoke(Player, (InventoryGroup)Enum.Parse(typeof(InventoryGroup), E.ToString()), y, f);
            OnInventoryRemoved.TryInvoke(Player, (InventoryGroup)Enum.Parse(typeof(InventoryGroup), E.ToString()), y, f);
        }

        public delegate void PlayerInventoryAdded(UnturnedPlayer player, InventoryGroup inventoryGroup, byte inventoryIndex, ItemJar P);
        public static event PlayerInventoryAdded OnPlayerInventoryAdded;
        public event PlayerInventoryAdded OnInventoryAdded;

        private void onInventoryAdded(byte E, byte u, ItemJar J)
        {
            OnPlayerInventoryAdded.TryInvoke(Player,(InventoryGroup)Enum.Parse(typeof(InventoryGroup), E.ToString()), u, J);
            OnInventoryAdded.TryInvoke(Player, (InventoryGroup)Enum.Parse(typeof(InventoryGroup), E.ToString()), u, J);
        }

        public delegate void PlayerChatted(UnturnedPlayer player, ref Color color, string message, EChatMode chatMode, ref bool cancel);
        public static event PlayerChatted OnPlayerChatted;

        internal static Color firePlayerChatted(UnturnedPlayer player, EChatMode chatMode, Color color, string msg, ref bool cancel)
        {
            if (OnPlayerChatted != null)
            {
                foreach (var handler in OnPlayerChatted.GetInvocationList().Cast<PlayerChatted>())
                {
                    try
                    {
                        handler(player, ref color, msg, chatMode, ref cancel);
                    }
                    catch (Exception ex)
                    {
                        Core.Logging.Logger.LogException(ex);
                    }
                }
            }

            return color;
        }

        public enum Wearables { Hat = 0, Mask = 1, Vest = 2, Pants = 3, Shirt = 4, Glasses = 5, Backpack = 6};
        public delegate void PlayerWear(UnturnedPlayer player, Wearables wear, ushort id, byte? quality);
        public static event PlayerWear OnPlayerWear;

    }
}
