using System.Collections.Generic;
using OpenMod.API.Permissions;
using Rocket.API.Serialisation;

namespace Rocket.PermissionLink
{
    public class RocketPermissionGroupWrapper : IPermissionGroup
    {
        public RocketPermissionGroupWrapper(RocketPermissionsGroup @group)
        {
            Id = group.Id;
            Type = "group";
            DisplayName = @group.DisplayName;
            Priority = 0;
            Parents = new HashSet<string>();
            IsAutoAssigned = false;

            if (!string.IsNullOrEmpty(@group.ParentGroup))
            {
                Parents.Add(@group.ParentGroup);
            }
        }

        public string Id { get; }
        public string Type { get; }
        public string DisplayName { get; }
        public int Priority { get; set; }
        public HashSet<string> Parents { get; }
        public bool IsAutoAssigned { get; set; }
    }
}