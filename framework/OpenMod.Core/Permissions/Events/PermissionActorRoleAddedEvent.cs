using OpenMod.API.Permissions;

namespace OpenMod.Core.Permissions.Events
{
    public class PermissionActorRoleAddedEvent : PermissionActorRoleEvent
    {
        public PermissionActorRoleAddedEvent(IPermissionActor actor, string roleId) : base(actor, roleId)
        {
        }
    }
}