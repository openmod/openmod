using OpenMod.API.Permissions;
using System;

namespace OpenMod.Core.Permissions
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = true)]
    public class RegisterPermissionAttribute : Attribute
    {
        public string Permission { get; }

        public string? Description { get; set; }

        public PermissionGrantResult DefaultGrant { get; set; }

        public RegisterPermissionAttribute(string permission)
        {
            Permission = permission;
            DefaultGrant = PermissionGrantResult.Default;
        }
    }
}
