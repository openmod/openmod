using System.Collections.Generic;
using OpenMod.API.Permissions;
using OpenMod.API.Users;

namespace OpenMod.Core.Permissions
{
    public class UserDataPermissionActor : IPermissionActor
    {
        public UserDataPermissionActor(UserData userData)
        {
            Id = userData.Id;
            Type = userData.Type;
            Data = userData.Data;
            DisplayName = userData.LastDisplayName;
        }

        public string Id { get; }

        public string Type { get; }

        public Dictionary<string, object> Data { get; }

        public string DisplayName { get; }

        public static implicit operator UserDataPermissionActor(UserData d) => new UserDataPermissionActor(d);
    }
}