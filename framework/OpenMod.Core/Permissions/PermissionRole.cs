using System;
using System.Collections.Generic;
using OpenMod.API;
using OpenMod.API.Permissions;
using OpenMod.Core.Permissions.Data;

namespace OpenMod.Core.Permissions
{
    [OpenModInternal]
    public sealed class PermissionRole : IPermissionRole
    {
        public PermissionRole(PermissionRoleData data)
        {
            Id = data.Id ?? throw new ArgumentException("Permission role is missing ID!");
            Priority = data.Priority;
            DisplayName = data.DisplayName ?? data.Id;
            Parents = data.Parents ?? new HashSet<string>();
            IsAutoAssigned = data.IsAutoAssigned;
            Permissions = data.Permissions ?? new HashSet<string>();
            FullActorName = $"role/{Id} ({DisplayName})";
        }

        public static implicit operator PermissionRole(PermissionRoleData data)
        {
            return new(data);
        }

        public static explicit operator PermissionRoleData(PermissionRole role)
        {
            return new()
            {
                Id = role.Id,
                Priority = role.Priority,
                DisplayName = role.DisplayName,
                Parents = role.Parents,
                IsAutoAssigned = role.IsAutoAssigned,
                Permissions = role.Permissions
            };
        }

        public string Id { get; } 
        public int Priority { get; set; }
        public string DisplayName { get; }

        public string FullActorName { get; }
        public HashSet<string> Parents { get; }
        public HashSet<string> Permissions { get; }
        public bool IsAutoAssigned { get; set; }
        public string Type { get; } = "role";
    }
}