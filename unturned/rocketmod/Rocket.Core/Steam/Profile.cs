using Rocket.Core.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Xml;

namespace Rocket.Core.Steam
{
    public class Profile
    {
        public ulong SteamID64 { get; set; }
        public string SteamID { get; set; }
        public string OnlineState { get; set; }
        public string StateMessage { get; set; }
        public string PrivacyState { get; set; }
        public ushort? VisibilityState { get; set; }
        public Uri AvatarIcon { get; set; }
        public Uri AvatarMedium { get; set; }
        public Uri AvatarFull { get; set; }
        public bool? IsVacBanned { get; set; }
        public string TradeBanState { get; set; }
        public bool? IsLimitedAccount { get; set; }
        public string CustomURL { get; set; }
        public DateTime? MemberSince { get; set; }
        public double? HoursPlayedLastTwoWeeks { get; set; }
        public string Headline { get; set; }
        public string Location { get; set; }
        public string RealName { get; set; }
        public string Summary { get; set; }
        public List<MostPlayedGame> MostPlayedGames { get; set; }
        public List<Group> Groups { get; set; }

        public class MostPlayedGame
        {
            public string Name { get; set; }
            public Uri Link { get; set; }
            public Uri Icon { get; set; }
            public Uri Logo { get; set; }
            public Uri LogoSmall { get; set; }
            public double? HoursPlayed { get; set; }
            public double? HoursOnRecord { get; set; }
        }

        public class Group
        {
            public ulong? SteamID64 { get; set; }
            public bool IsPrimary { get; set; }
            public string Name { get; set; }
            public string URL { get; set; }
            public Uri AvatarIcon { get; set; }
            public Uri AvatarMedium { get; set; }
            public Uri AvatarFull { get; set; }
            public string Headline { get; set; }
            public string Summary { get; set; }
            public uint? MemberCount { get; set; }
            public uint? MembersInGame { get; set; }
            public uint? MembersInChat { get; set; }
            public uint? MembersOnline { get; set; }
        }

        public Profile(ulong steamID64)
        {
            SteamID64 = steamID64;
            Reload();
        }


        public void Reload()
        {
            string field = "unknown";
            try
            {

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(new WebClient().DownloadString("http://steamcommunity.com/profiles/" + SteamID64 + "?xml=1"));

                SteamID = doc["profile"]["steamID"]?.ParseString(); field = "SteamID";
                OnlineState = doc["profile"]["onlineState"]?.ParseString(); field = "OnlineState";
                StateMessage = doc["profile"]["stateMessage"]?.ParseString(); field = "StateMessage";
                PrivacyState = doc["profile"]["privacyState"]?.ParseString(); field = "PrivacyState";
                VisibilityState = doc["profile"]["visibilityState"]?.ParseUInt16(); field = "VisibilityState";
                AvatarIcon = doc["profile"]["avatarIcon"]?.ParseUri(); field = "AvatarIcon";
                AvatarMedium = doc["profile"]["avatarMedium"]?.ParseUri(); field = "AvatarMedium";
                AvatarFull = doc["profile"]["avatarFull"]?.ParseUri(); field = "AvatarFull";
                IsVacBanned = doc["profile"]["vacBanned"]?.ParseBool(); field = "IsVacBanned";
                TradeBanState = doc["profile"]["tradeBanState"]?.ParseString(); field = "TradeBanState";
                IsLimitedAccount = doc["profile"]["isLimitedAccount"]?.ParseBool(); field = "IsLimitedAccount";
                 
                CustomURL = doc["profile"]["customURL"]?.ParseString(); field = "CustomURL";
                MemberSince = doc["profile"]["memberSince"]?.ParseDateTime(new CultureInfo("en-US", false)); field = "MemberSince";
                HoursPlayedLastTwoWeeks = doc["profile"]["hoursPlayed2Wk"]?.ParseDouble(); field = "HoursPlayedLastTwoWeeks";
                Headline = doc["profile"]["headline"]?.ParseString(); field = "Headline";
                Location = doc["profile"]["location"]?.ParseString(); field = "Location";
                RealName = doc["profile"]["realname"]?.ParseString(); field = "RealName";
                Summary = doc["profile"]["summary"]?.ParseString(); field = "Summary";

                if (doc["profile"]["mostPlayedGames"] != null)
                {
                    MostPlayedGames = new List<MostPlayedGame>(); field = "MostPlayedGames";
                    foreach (XmlElement mostPlayedGame in doc["profile"]["mostPlayedGames"].ChildNodes)
                    {
                        MostPlayedGame newMostPlayedGame = new MostPlayedGame();
                        newMostPlayedGame.Name = mostPlayedGame["gameName"]?.ParseString(); field = "MostPlayedGame.Name";
                        newMostPlayedGame.Link = mostPlayedGame["gameLink"]?.ParseUri(); field = "MostPlayedGame.Link";
                        newMostPlayedGame.Icon = mostPlayedGame["gameIcon"]?.ParseUri(); field = "MostPlayedGame.Icon";
                        newMostPlayedGame.Logo = mostPlayedGame["gameLogo"]?.ParseUri(); field = "MostPlayedGame.Logo";
                        newMostPlayedGame.LogoSmall = mostPlayedGame["gameLogoSmall"]?.ParseUri(); field = "MostPlayedGame.LogoSmall";
                        newMostPlayedGame.HoursPlayed = mostPlayedGame["hoursPlayed"]?.ParseDouble(); field = "MostPlayedGame.HoursPlayed";
                        newMostPlayedGame.HoursOnRecord = mostPlayedGame["hoursOnRecord"]?.ParseDouble(); field = "MostPlayedGame.HoursOnRecord";
                        MostPlayedGames.Add(newMostPlayedGame); 
                    }
                }

                if (doc["profile"]["groups"] != null)
                {
                    Groups = new List<Group>(); field = "Groups";
                    foreach (XmlElement group in doc["profile"]["groups"].ChildNodes)
                    {
                        Group newGroup = new Group();
                        newGroup.IsPrimary = group.Attributes["isPrimary"] != null && group.Attributes["isPrimary"].InnerText == "1"; field = "Group.IsPrimary";
                        newGroup.SteamID64 = group["groupID64"]?.ParseUInt64(); field = "Group.SteamID64";
                        newGroup.Name = group["groupName"]?.ParseString(); field = "Group.Name";
                        newGroup.URL = group["groupURL"]?.ParseString(); field = "Group.URL";
                        newGroup.Headline = group["headline"]?.ParseString(); field = "Group.Headline";
                        newGroup.Summary = group["summary"]?.ParseString(); field = "Group.Summary";
                        newGroup.AvatarIcon = group["avatarIcon"]?.ParseUri(); field = "Group.AvatarIcon";
                        newGroup.AvatarMedium = group["avatarMedium"]?.ParseUri(); field = "Group.AvatarMedium";
                        newGroup.AvatarFull = group["avatarFull"]?.ParseUri(); field = "Group.AvatarFull";
                        newGroup.MemberCount = group["memberCount"]?.ParseUInt32(); field = "Group.MemberCount";
                        newGroup.MembersInChat = group["membersInChat"]?.ParseUInt32(); field = "Group.MembersInChat";
                        newGroup.MembersInGame = group["membersInGame"]?.ParseUInt32(); field = "Group.MembersInGame";
                        newGroup.MembersOnline = group["membersOnline"]?.ParseUInt32(); field = "Group.MembersOnline";
                        Groups.Add(newGroup);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "Error reading Steam Profile, Field: " + field);
            }
        }
    }
    public static class XmlElementExtensions
    {
        public static string ParseString(this XmlElement element)
        {
            return element.InnerText;
        }


        public static DateTime? ParseDateTime(this XmlElement element, CultureInfo cultureInfo)
        {
            try
            {
                return element == null ? null : (DateTime?)DateTime.Parse(element.InnerText.Replace("st", "").Replace("nd", "").Replace("rd", "").Replace("th", ""), cultureInfo);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static double? ParseDouble(this XmlElement element)
        {
            try
            {
                return element == null ? null : (double?)double.Parse(element.InnerText);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static ushort? ParseUInt16(this XmlElement element)
        {
            try
            {
                return element == null ? null : (ushort?)ushort.Parse(element.InnerText);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static uint? ParseUInt32(this XmlElement element)
        {
            try
            {
                return element == null ? null : (uint?)uint.Parse(element.InnerText);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static ulong? ParseUInt64(this XmlElement element)
        {
            try
            {
                return element == null ? null : (ulong?)ulong.Parse(element.InnerText);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static bool? ParseBool(this XmlElement element)
        {
            try
            {
                return element == null ? null : (bool?)(element.InnerText == "1");
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static Uri ParseUri(this XmlElement element)
        {
            try
            {
                return element == null ? null : new Uri(element.InnerText);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}