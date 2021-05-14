using OpenMod.API.Permissions;

namespace OpenMod.Core.Permissions.Events
{
    /// <summary>
    /// Event that is triggered when a role is added to the actor
    /// </summary>
    public class PermissionActorRoleAddedEvent : PermissionActorRoleEvent
    {
        public PermissionActorRoleAddedEvent(IPermissionActor actor, string roleId) : base(actor, roleId)
        {
        }
    }
}