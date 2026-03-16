using System;
using System.Collections.Generic;
using VYaml.Annotations;

namespace OpenMod.Core.Permissions.Data
{
    [Serializable, YamlObject]
    public sealed partial class PermissionRolesData
    {
        public List<PermissionRoleData>? Roles { get; set; }
    }
}