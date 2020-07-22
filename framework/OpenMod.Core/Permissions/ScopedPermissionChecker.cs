using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Permissions;

namespace OpenMod.Core.Permissions
{
    public class ScopedPermissionChecker : IPermissionChecker
    {
        private readonly IPermissionChecker m_Parent;
        private readonly IOpenModComponent m_Component;

        public ScopedPermissionChecker(IRuntime runtime, IOpenModComponent component)
        {
            m_Parent = runtime.Host.Services.GetRequiredService<IPermissionChecker>();
            m_Component = component;
        }

        public IReadOnlyCollection<IPermissionCheckProvider> PermissionCheckProviders
        {
            get { return m_Parent.PermissionCheckProviders; }
        }

        public IReadOnlyCollection<IPermissionStore> PermissionStores
        {
            get { return m_Parent.PermissionStores; }
        }

        public Task<PermissionGrantResult> CheckPermissionAsync(IPermissionActor actor, string permission)
        {
            return m_Parent.CheckPermissionAsync(actor, m_Component.OpenModComponentId + "." + permission);
        }

        public Task InitAsync()
        {
            return m_Parent.InitAsync();
        }
    }
}