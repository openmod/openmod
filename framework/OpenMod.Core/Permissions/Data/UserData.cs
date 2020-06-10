using System;
using System.Collections.Generic;

namespace OpenMod.Core.Permissions.Data
{
    [Serializable]
    public class UserData
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string LastDisplayName { get; set; }
        public DateTime FirstSeen { get; set; }
        public DateTime LastSeen { get; set; }
        public List<string> Permissions { get; set; }
        public List<string> Groups { get; set; }
        public Dictionary<string, object> Data { get; set; }
    }
}