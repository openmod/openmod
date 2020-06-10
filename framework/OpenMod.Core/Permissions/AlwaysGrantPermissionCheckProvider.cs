using System;
using System.Threading.Tasks;
using OpenMod.API.Permissions;
using OpenMod.API.Prioritization;

namespace OpenMod.Core.Permissions
{
    [Priority(Priority = Priority.Highest)]
    public class AlwaysGrantPermissionCheckProvider : IPermissionCheckProvider
    {
        private readonly Func<IPermissionActor, bool> m_ActorPredicate;

        public AlwaysGrantPermissionCheckProvider(Func<IPermissionActor, bool> actorPredicate)
        {
            m_ActorPredicate = actorPredicate;
        }
        public bool SupportsActor(IPermissionActor actor)
        {
            return m_ActorPredicate(actor);
        }

        public Task<PermissionGrantResult> CheckPermissionAsync(IPermissionActor actor, string permission)
        {
            return Task.FromResult(PermissionGrantResult.Grant);
        }
    }
}