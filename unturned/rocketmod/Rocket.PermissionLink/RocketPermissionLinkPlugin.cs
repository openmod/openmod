using System;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Permissions;
using OpenMod.Core.Plugins;
using OpenMod.UnityEngine.Plugins;

[assembly: PluginMetadata(Author = "OpenMod", DisplayName = "Rocket Permission Link", Id = "Rocket.PermissionLink")]

namespace Rocket.PermissionLink
{
    public class RocketPermissionLinkPlugin : OpenModUnityEnginePlugin
    {
        private readonly IServiceProvider m_ServiceProvider;
        private OpenModPermissionsProvider m_PermissionProvider;

        public RocketPermissionLinkPlugin(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            m_ServiceProvider = serviceProvider;
        }

        protected override UniTask OnLoadAsync()
        {
            m_PermissionProvider = ActivatorUtilities.CreateInstance<OpenModPermissionsProvider>(m_ServiceProvider);
            m_PermissionProvider.Install();
            return base.OnLoadAsync();
        }

        protected override UniTask OnUnloadAsync()
        {
            m_PermissionProvider.Dispose();
            return base.OnUnloadAsync();
        }
    }
}
