using OpenMod.API.Permissions;

namespace OpenMod.Core.Permissions.Events
{
    public class PermissionRoleCreatedEvent : PermissionRoleEvent
    {
        public PermissionRoleCreatedEvent(IPermissionRole permissionRole) : base(permissionRole)
        {
        }
    }
}