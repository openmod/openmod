using OpenMod.API.Permissions;

namespace OpenMod.Core.Permissions.Events
{
    public class PermissionRoleUpdatedEvent : PermissionRoleEvent
    {
        public PermissionRoleUpdatedEvent(IPermissionRole permissionRole) : base(permissionRole)
        {
        }
    }
}