using OpenMod.API.Permissions;

namespace OpenMod.Core.Permissions.Events
{
    public class PermissionActorRoleRemovedEvent : PermissionActorRoleEvent
    {
        public PermissionActorRoleRemovedEvent(IPermissionActor actor, string roleId) : base(actor, roleId)
        {
        }
    }
}