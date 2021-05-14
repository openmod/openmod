using OpenMod.API.Permissions;

namespace OpenMod.Core.Permissions.Events
{
    /// <summary>
    /// Event that is triggered when a role is removed from the actor
    /// </summary>
    public class PermissionActorRoleRemovedEvent : PermissionActorRoleEvent
    {
        public PermissionActorRoleRemovedEvent(IPermissionActor actor, string roleId) : base(actor, roleId)
        {
        }
    }
}