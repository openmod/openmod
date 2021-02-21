using System.Collections.Generic;
using OpenMod.API.Permissions;
using Rocket.API.Serialisation;

namespace OpenMod.Unturned.RocketMod.Permissions
{
    public class RocketGroupWrapper : IPermissionRole
    {
        public RocketGroupWrapper(RocketPermissionsGroup rocketGroup)
        {
            Id = rocketGroup.Id;
            Type = "role";
            DisplayName = rocketGroup.DisplayName;
            Priority = rocketGroup.Priority;
            Parents = !string.IsNullOrEmpty(rocketGroup.ParentGroup)
                ? new HashSet<string> { rocketGroup.ParentGroup }
                : new HashSet<string>();

            IsAutoAssigned = false; // todo check if default role
            FullActorName = $"rocketmodGroup/{Id} ({DisplayName})";
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