using OpenMod.API.Permissions;
using System;

namespace OpenMod.Core.Permissions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class RegisterCommandPermissionAttribute : Attribute
    {
        public string Permission { get; }
        
        public string? Description { get; set; }

        public PermissionGrantResult DefaultGrant { get; set; }

        public RegisterCommandPermissionAttribute(string permission)
        {
            Permission = permission;
            DefaultGrant = PermissionGrantResult.Default;
        }
    }
}