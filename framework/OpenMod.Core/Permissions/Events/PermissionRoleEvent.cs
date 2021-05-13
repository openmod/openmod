using OpenMod.API.Permissions;
using OpenMod.Core.Eventing;

namespace OpenMod.Core.Permissions.Events
{
    public abstract class PermissionRoleEvent : Event
    {
        public IPermissionRole PermissionRole { get; }

        public PermissionRoleEvent(IPermissionRole permissionRole)
            => PermissionRole = permissionRole;
    }
}