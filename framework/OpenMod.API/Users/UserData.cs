using System;
using System.Collections.Generic;

namespace OpenMod.API.Users
{
    [Serializable]
    public class UserData
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string LastDisplayName { get; set; }
        public DateTime FirstSeen { get; set; }
        public DateTime LastSeen { get; set; }
        public HashSet<string> Permissions { get; set; }
        public HashSet<string> Roles { get; set; }
        public Dictionary<string, object> Data { get; set; }


        public UserData()
        {
            Data = new Dictionary<string, object>();
            Roles = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
            Permissions = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
        }
    }
}