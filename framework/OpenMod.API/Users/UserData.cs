using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace OpenMod.API.Users
{
    /// <summary>
    /// Serialized user data.
    /// </summary>
    [Serializable]
    public class UserData
    {
        /// <value>
        /// The ID of the user.
        /// </value>
        public string Id { get; set; }

        /// <value>
        /// The actor type of the user.
        /// </value>
        public string Type { get; set; }

        /// <value>
        /// The last known display name of the user.
        /// </value>
        public string LastDisplayName { get; set; }

        /// <value>
        /// The first time the user has been seen.
        /// </value>
        public DateTime FirstSeen { get; set; }

        /// <value>
        /// The last time the user has been seen.
        /// </value>
        public DateTime LastSeen { get; set; }

        /// <value>
        /// The permissions of the user. Cannot be null and neither can items be null.
        /// </value>
        [NotNull]
        [ItemNotNull]
        public HashSet<string> Permissions { get; set; }

        /// <value>
        /// The roles of the user. Cannot be null and neither can items be null.
        /// </value>
        public HashSet<string> Roles { get; set; }

        /// <value>
        /// The user data. Can be null. Plugins should use <see cref="IUserDataStore"/> to interact with this data.
        /// </value>
        public Dictionary<string, object> Data { get; set; }

        public UserData()
        {
            Data = new Dictionary<string, object>();
            Roles = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
            Permissions = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
        }
    }
}