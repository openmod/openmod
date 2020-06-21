using System;
using System.Collections.Generic;
using OpenMod.API.Permissions;
using OpenMod.Core.Permissions.Data;

namespace OpenMod.Core.Permissions
{
    public sealed class PermissionRole : IPermissionRole
    {
        public PermissionRole()
        {
            Parents = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
        }
        public PermissionRole(PermissionRoleData data)
        {
            Id = data.Id;
            Priority = data.Priority;
            DisplayName = data.DisplayName;
            Parents = data.Parents;
            IsAutoAssigned = data.IsAutoAssigned;
            Permissions = data.Permissions;
        }

        public static implicit operator PermissionRole(PermissionRoleData data)
        {
            return new PermissionRole(data);
        }

        public static explicit operator PermissionRoleData(PermissionRole role)
        {
            return new PermissionRoleData
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
        public HashSet<string> Parents { get; }
        public HashSet<string> Permissions { get; }
        public bool IsAutoAssigned { get; set; }
        public string Type { get; } = "role";
    }
}