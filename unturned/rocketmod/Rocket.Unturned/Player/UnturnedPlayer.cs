using SDG.Unturned;
using Steamworks;
using System;
using UnityEngine;
using System.Linq;
using Rocket.Unturned.Events;
using Rocket.API;
using Rocket.Core;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Skills;
using Rocket.Core.Steam;
using Rocket.API.Serialisation;

namespace Rocket.Unturned.Player
{
    public class PlayerIsConsoleException : Exception { }

    public sealed class UnturnedPlayer : IRocketPlayer
    {

        public string Id
        {
            get
            {
                return CSteamID.ToString();
            }
        }

        public string DisplayName
        {
            get
            {
                return CharacterName;
            }
        }

        public bool IsAdmin
        {
            get
            {
                return player.channel.owner.isAdmin;
            }
        }

        public Profile SteamProfile
        {
            get { return new Profile(ulong.Parse(CSteamID.ToString())); }
        }

        private SDG.Unturned.Player player;
        public SDG.Unturned.Player Player
        {
            get { return player; }
        }

        public CSteamID CSteamID
        {
            get { return player.channel.owner.playerID.steamID; }
        }

        public Exception PlayerIsConsoleException;

        private UnturnedPlayer(SteamPlayer player)
        {
            this.player = player.player;
        }

        public Color Color
        {
            get
            {
                if (Features.Color.HasValue)
                {
                    return Features.Color.Value;
                }
                if (IsAdmin && !Provider.hideAdmins)
                {
                    return Palette.ADMIN;
                }

                RocketPermissionsGroup group = R.Permissions.GetGroups(this,false).Where(g => g.Color != null && g.Color != "white").FirstOrDefault();
                string color = "";
                if (group != null) color = group.Color;
                return UnturnedChat.GetColorFromName(color, Palette.COLOR_W);
            }
            set
            {
                Features.Color = value;
            }
        }


        private UnturnedPlayer(CSteamID cSteamID)
        {
            if (string.IsNullOrEmpty(cSteamID.ToString()) || cSteamID.ToString() == "0")
            {
                throw new PlayerIsConsoleException();
            }
            else
            {
                player = PlayerTool.getPlayer(cSteamID);
            }
        }

        public float Ping
        {
            get
            {
                return player.channel.owner.ping;
            }
        }

        public bool Equals(UnturnedPlayer otherPlayer)
        {
            if(ReferenceEquals(otherPlayer, null))
            {
                return false;
            }
            else
            {
                return this.CSteamID == otherPlayer.CSteamID;
            }
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as UnturnedPlayer);
        }

        public override int GetHashCode()
        {
            return this.CSteamID.GetHashCode();
        }

        public T GetComponent<T>()
        {
            return (T)(object)Player.GetComponent(typeof(T));
        }

        private UnturnedPlayer(SDG.Unturned.Player p)
        {
            player = p;
        }

        public static UnturnedPlayer FromName(string name)
        {
            if (String.IsNullOrEmpty(name)) return null;
            SDG.Unturned.Player p = null;
            ulong id = 0;
            if (ulong.TryParse(name, out id) && id > 76561197960265728)
            {
                p = PlayerTool.getPlayer(new CSteamID(id));
            }
            else
            {
                p = PlayerTool.getPlayer(name);
            }
            if (p == null) return null;
            return new UnturnedPlayer(p);
        }

        public static UnturnedPlayer FromCSteamID(CSteamID cSteamID)
        {
            if (string.IsNullOrEmpty(cSteamID.ToString()) || cSteamID.ToString() == "0")
            {
                return null;
            }
            else
            {
                return new UnturnedPlayer(cSteamID);
            }
        }

        public static UnturnedPlayer FromPlayer(SDG.Unturned.Player player)
        {
            return new UnturnedPlayer(player.channel.owner);
        }

        public static UnturnedPlayer FromSteamPlayer(SteamPlayer player)
        {
            return new UnturnedPlayer(player);
        }

        public UnturnedPlayerFeatures Features
        {
            get { return player.gameObject.transform.GetComponent<UnturnedPlayerFeatures>(); }
        }

        public UnturnedPlayerEvents Events
        {
            get { return player.gameObject.transform.GetComponent<UnturnedPlayerEvents>(); }
        }

        public override string ToString()
        {
            return CSteamID.ToString();
        }

        public void TriggerEffect(ushort effectID)
        {
            EffectManager.instance.channel.send("tellEffectPoint", CSteamID, ESteamPacket.UPDATE_UNRELIABLE_BUFFER, new object[] { effectID, player.transform.position });
        }
        
        public string IP
        {
            get
            {
                P2PSessionState_t State;
                SteamGameServerNetworking.GetP2PSessionState(CSteamID, out State);
                return Parser.getIPFromUInt32(State.m_nRemoteIP);
            }
        }

        public void MaxSkills()
        {
            PlayerSkills skills = player.skills;
            
            foreach (var skill in skills.skills.SelectMany(s => s))
            {
                skill.level = skill.max;
            }
            
            skills.askSkills(player.channel.owner.playerID.steamID);
        }

        public string SteamGroupName()
        {
            FriendsGroupID_t id;
            id.m_FriendsGroupID = (short)SteamGroupID.m_SteamID;
            return SteamFriends.GetFriendsGroupName(id);
        }

        public int SteamGroupMembersCount()
        {
            FriendsGroupID_t id;
            id.m_FriendsGroupID = (short)SteamGroupID.m_SteamID;
            return SteamFriends.GetFriendsGroupMembersCount(id);
        }

        public SteamPlayer SteamPlayer()
        {
            foreach (var SteamPlayer in Provider.clients)
            {
                if (CSteamID == SteamPlayer.playerID.steamID)
                {
                    return SteamPlayer;
                }
            }
            return null;
        }

        public PlayerInventory Inventory
        {
            get { return player.inventory; }
        }

        public bool GiveItem(ushort itemId, byte amount)
        {
            return ItemTool.tryForceGiveItem(player, itemId, amount);
        }

        public bool GiveItem(Item item)
        {
            return player.inventory.tryAddItem(item, false);
        }

        public bool GiveVehicle(ushort vehicleId)
        {
            return VehicleTool.giveVehicle(player, vehicleId);
        }

        public CSteamID SteamGroupID
        {
            get
            {
                return player.channel.owner.playerID.group;
            }
        }

        public void Kick(string reason)
        {
            Provider.kick(this.CSteamID, reason);
        }

        public void Ban(string reason, uint duration)
        {
            Ban(CSteamID.Nil, reason, duration);
        }

        public void Ban(CSteamID instigator, string reason, uint duration)
        {
            CSteamID steamIdToBan = this.CSteamID;

            uint ipToBan = 0;
            P2PSessionState_t state;
            if(SteamGameServerNetworking.GetP2PSessionState(steamIdToBan, out state))
            {
                ipToBan = state.m_nRemoteIP;
            }

            Provider.requestBanPlayer(instigator, steamIdToBan, ipToBan, reason, duration);
        }

        public void Admin(bool admin)
        {
            Admin(admin, null);
        }

        public void Admin(bool admin, UnturnedPlayer issuer)
        {
            if (admin)
            {
                if (issuer == null)
                {
                    SteamAdminlist.admin(this.CSteamID, new CSteamID(0));
                }
                else
                {
                    SteamAdminlist.admin(this.CSteamID, issuer.CSteamID);
                }
            }
            else
            {
                SteamAdminlist.unadmin(player.channel.owner.playerID.steamID);
            }
        }

        public void Teleport(UnturnedPlayer target)
        {
            Vector3 d1 = target.player.transform.position;
            Vector3 vector31 = target.player.transform.rotation.eulerAngles;
            Teleport(d1, MeasurementTool.angleToByte(vector31.y));
        }

        public void Teleport(Vector3 position, float rotation)
        {
            if (VanishMode)
            {
                player.channel.send("askTeleport", ESteamCall.OWNER, ESteamPacket.UPDATE_RELIABLE_BUFFER, position, MeasurementTool.angleToByte(rotation));
                player.channel.send("askTeleport", ESteamCall.NOT_OWNER, ESteamPacket.UPDATE_RELIABLE_BUFFER, new Vector3(position.y, position.y + 1337, position.z), MeasurementTool.angleToByte(rotation));
                player.channel.send("askTeleport", ESteamCall.SERVER, ESteamPacket.UPDATE_RELIABLE_BUFFER, position, MeasurementTool.angleToByte(rotation));
            }
            else
            {
				player.teleportToLocation(position, rotation);
            }
        }

        public bool VanishMode
        {
            get
            {
                UnturnedPlayerFeatures features = player.GetComponent<UnturnedPlayerFeatures>();
                return features.VanishMode;
            }
            set
            {
                UnturnedPlayerFeatures features = player.GetComponent<UnturnedPlayerFeatures>();
                features.VanishMode = value;
            }
        }

        public bool GodMode
        {
            get
            {
                UnturnedPlayerFeatures features = player.GetComponent<UnturnedPlayerFeatures>();
                return features.GodMode;
            }
            set
            {
                UnturnedPlayerFeatures features = player.GetComponent<UnturnedPlayerFeatures>();
                features.GodMode = value;
            }
        }

        public Vector3 Position
        {
            get
            {
                return player.transform.position;
            }
        }

        public EPlayerStance Stance
        {
            get
            {
                return player.stance.stance;
            }
        }

        public float Rotation
        {
            get
            {
                return player.transform.rotation.eulerAngles.y;
            }
        }

        public bool Teleport(string nodeName)
        {
            Node node = LevelNodes.nodes.Where(n => n.type == ENodeType.LOCATION && ((LocationNode)n).name.ToLower().Contains(nodeName)).FirstOrDefault();
            if (node != null)
            {
                Vector3 c = node.point + new Vector3(0f, 0.5f, 0f);
                player.sendTeleport(c, MeasurementTool.angleToByte(Rotation));
                return true;
            }
            return false;
        }

        public byte Stamina
        {
            get
            {
                return player.life.stamina;
            }
        }

        public string CharacterName
        {
            get
            {
                return player.channel.owner.playerID.characterName;
            }
        }

        public string SteamName
        {
            get
            {
                return player.channel.owner.playerID.playerName;
            }
        }

        public byte Infection
        {
            get
            {
                return player.life.virus;
            }
            set
            {
                player.life.askDisinfect(100);
                player.life.askInfect(value);
            }
        }

        public uint Experience
        {
            get
            {
                return player.skills.experience;
            }
            set
            {
                player.skills.channel.send("tellExperience", ESteamCall.SERVER, ESteamPacket.UPDATE_RELIABLE_BUFFER, value);
                player.skills.channel.send("tellExperience", ESteamCall.OWNER, ESteamPacket.UPDATE_RELIABLE_BUFFER, value);
            }
        }

        public int Reputation
        {
            get
            {
                return player.skills.reputation;
            }
            set
            {
                player.skills.askRep(value);
            }
        }

        public byte Health
        {
            get
            {
                return player.life.health;
            }
        }

        public byte Hunger
        {
            get
            {
                return player.life.food;
            }
            set
            {
                player.life.askEat(100);
                player.life.askStarve(value);
            }
        }

        public byte Thirst
        {
            get
            {
                return player.life.water;
            }
            set
            {
                player.life.askDrink(100);
                player.life.askDehydrate(value);
            }
        }

        public bool Broken
        {
            get
            {
                return player.life.isBroken;
            }
            set
            {
                player.life.tellBroken(Provider.server,value);
                player.life.channel.send("tellBroken", ESteamCall.OWNER, ESteamPacket.UPDATE_RELIABLE_BUFFER, new object[] { value });
            }
        }
        public bool Bleeding
        {
            get
            {
                return player.life.isBleeding;
            }
            set
            {
                player.life.tellBleeding(Provider.server, value);
                player.life.channel.send("tellBleeding", ESteamCall.OWNER, ESteamPacket.UPDATE_RELIABLE_BUFFER, new object[] { value });
            }
        }

        public bool Dead
        {
            get
            {
                return player.life.isDead;
            }
        }

        public void Heal(byte amount)
        {
            Heal(amount, null, null);
        }

        public void Heal(byte amount, bool? bleeding, bool? broken)
        {
            player.life.askHeal(amount, bleeding != null ? bleeding.Value : player.life.isBleeding, broken != null ? broken.Value : player.life.isBroken);
        }

        public void Suicide()
        {
            player.life.askSuicide(player.channel.owner.playerID.steamID);
        }

        public EPlayerKill Damage(byte amount, Vector3 direction, EDeathCause cause, ELimb limb, CSteamID damageDealer)
        {
            EPlayerKill playerKill;
            player.life.askDamage(amount, direction, cause, limb, damageDealer, out playerKill);
            return playerKill;
        }

        public bool IsPro
        {
            get
            {
                return player.channel.owner.isPro;
            }
        }

        public InteractableVehicle CurrentVehicle
        {
            get
            {
                return player.movement.getVehicle();
            }
        }

        public bool IsInVehicle
        {
            get
            {
                return CurrentVehicle != null;
            }
        }

        public void SetSkillLevel(UnturnedSkill skill, byte level)
        {
            GetSkill(skill).level = level;
            player.skills.askSkills(CSteamID);
        }

        public byte GetSkillLevel(UnturnedSkill skill)
        {
            return GetSkill(skill).level;
        }

        public Skill GetSkill(UnturnedSkill skill)
        {
            var skills = player.skills;
            return skills.skills[skill.Speciality][skill.Skill];
        }

        public int CompareTo(object obj)
        {
            return Id.CompareTo(obj);
        }
    }
}
