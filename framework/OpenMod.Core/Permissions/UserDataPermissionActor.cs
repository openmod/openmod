using System;
using System.Collections.Generic;
using OpenMod.API.Permissions;
using OpenMod.API.Users;

namespace OpenMod.Core.Permissions
{
    public class UserDataPermissionActor : IPermissionActor
    {
        public UserDataPermissionActor(UserData userData)
        {
            Id = userData.Id ?? throw new InvalidOperationException("userData.Id was null!");
            Type = userData.Type ?? throw new InvalidOperationException("userData.Type was null!"); ;
            Data = userData.Data;
            DisplayName = userData.LastDisplayName ?? Id;
            FullActorName = $"{Type}/{Id} ({DisplayName})";
        }

        public string Id { get; }

        public string Type { get; }

        public Dictionary<string, object?>? Data { get; }

        public string DisplayName { get; }

        public string FullActorName { get; }

        public static implicit operator UserDataPermissionActor(UserData d) => new(d);
    }
}