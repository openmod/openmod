using System;
using System.Threading.Tasks;
using OpenMod.API.Permissions;
using OpenMod.API.Prioritization;

namespace OpenMod.Core.Permissions
{
    [Priority(Priority = Priority.Highest)]
    public class AlwaysDenyPermissionCheckProvider : IPermissionCheckProvider
    {
        private readonly Func<IPermissionActor, bool> m_ActorPredicate;

        public AlwaysDenyPermissionCheckProvider(Func<IPermissionActor, bool> actorPredicate)
        {
            m_ActorPredicate = actorPredicate;
        }
        
        public bool SupportsActor(object actor)
        {
            return actor is IPermissionActor act && m_ActorPredicate(act);
        }

        public Task<PermissionGrantResult> CheckPermissionAsync(IPermissionActor actor, string permission)
        {
            return Task.FromResult(PermissionGrantResult.Deny);
        }
    }
}