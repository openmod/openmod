using System;
using System.Collections.Generic;

namespace OpenMod.Core.Permissions.Data
{
    [Serializable]
    public class PermissionRoleData
    {
        public string Id { get; set; }
        public int Priority { get; set; }
        public HashSet<string> Parents { get; set; }
        public HashSet<string> Permissions { get; set; }
        public string DisplayName { get; set; }
        public Dictionary<string, object> Data { get; set; }
        public bool IsAutoAssigned { get; set; }


        public PermissionRoleData()
        {
            Data = new Dictionary<string, object>();
            Parents = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
            Permissions = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
        }
    }
}