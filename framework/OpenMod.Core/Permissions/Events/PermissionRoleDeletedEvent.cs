using OpenMod.API.Permissions;

namespace OpenMod.Core.Permissions.Events
{
    public class PermissionRoleDeletedEvent : PermissionRoleEvent {
        public PermissionRoleDeletedEvent(IPermissionRole permissionRole) : base(permissionRole)
        {
        }
    }
}