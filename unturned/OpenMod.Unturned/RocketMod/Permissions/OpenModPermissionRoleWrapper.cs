using System.Collections.Generic;
using OpenMod.API.Permissions;
using Rocket.API.Serialisation;

namespace OpenMod.Unturned.RocketMod.Permissions
{
    public class OpenModPermissionRoleWrapper : IPermissionRole
    {
        public OpenModPermissionRoleWrapper(RocketPermissionsGroup @group)
        {
            Id = group.Id;
            Type = "role";
            DisplayName = @group.DisplayName;
            Priority = 0;
            Parents = new HashSet<string>();
            IsAutoAssigned = false;

            if (!string.IsNullOrEmpty(@group.ParentGroup))
            {
                Parents.Add(@group.ParentGroup);
            }

            FullActorName = $"rocketmodRole/{Id} ({DisplayName})";
        }

        public string Id { get; }
        public string Type { get; }
        public string DisplayName { get; }
        public string FullActorName { get; }
        public int Priority { get; set; }
        public HashSet<string> Parents { get; }
        public bool IsAutoAssigned { get; set; }
    }
}