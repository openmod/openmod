using OpenMod.API.Permissions;
using OpenMod.Core.Eventing;

namespace OpenMod.Core.Permissions.Events
{
    /// <summary>
    /// Event that is triggered when a role is added to or removed from the actor
    /// </summary>
    public abstract class PermissionActorRoleEvent : Event
    {

        public IPermissionActor Actor { get; }

        public string RoleId { get; }

        public PermissionActorRoleEvent(IPermissionActor actor, string roleId)
        {
            Actor = actor;
            RoleId = roleId;
        }
    }
}