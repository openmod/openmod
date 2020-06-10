using System;
using System.Collections.Generic;

namespace OpenMod.Core.Permissions.Data
{
    [Serializable]
    public class PermissionGroupsData
    {
        public List<PermissionGroupData> PermissionGroups { get; set; }
    }
}