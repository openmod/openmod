using System;
using System.Collections.Generic;

namespace OpenMod.Core.Permissions.Data
{
    [Serializable]
    public class PermissionGroupData
    {
        public string Id { get; set; }
        public int Priority { get; set; }
        public List<string> Parents { get; set; }
        public List<string> Permissions { get; set; }
        public string DisplayName { get; set; }
        public Dictionary<string, object> Data { get; set; }
        public bool IsAutoAssigned { get; set; }
    }
}