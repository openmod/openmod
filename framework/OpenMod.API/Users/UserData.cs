using System;
using System.Collections.Generic;

namespace OpenMod.API.Users
{
    /// <summary>
    /// Serialized user data.
    /// </summary>
    [Serializable]
    public sealed class UserData
    {
        /// <summary>
        /// Gets or sets the ID of the user.
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Gets or sets the actor type of the user.
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// Gets or sets the last known display name of the user.
        /// </summary>
        public string? LastDisplayName { get; set; }

        /// <summary>
        /// Gets or sets the first time the user has been seen.
        /// </summary>
        public DateTime? FirstSeen { get; set; }

        /// <summary>
        /// Gets or sets the last time the user has been seen.
        /// </summary>
        public DateTime? LastSeen { get; set; }

        /// <summary>
        /// Gets the data related to user ban.
        /// </summary>
        public BanData? BanInfo { get; set; }

        /// <summary>
        /// Gets or sets the permissions of the user.
        /// </summary>
        public HashSet<string>? Permissions { get; set; }

        /// <summary>
        /// Gets or sets the roles of the user.
        /// </summary>
        public HashSet<string>? Roles { get; set; }

        /// <summary>
        /// Gets the user data. Plugins should use the <see cref="IUserDataStore"/> service to interact with this data.
        /// </summary>
        public Dictionary<string, object?>? Data { get; set; }

        public UserData()
        {
            Data = new Dictionary<string, object?>();
            Roles = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
            Permissions = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
        }

        public UserData(string id, string type) : this()
        {
            Id = id;
            Type = type;
        }
    }
}
