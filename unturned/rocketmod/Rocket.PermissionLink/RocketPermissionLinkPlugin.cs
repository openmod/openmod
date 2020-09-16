using System;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Plugins;
using OpenMod.UnityEngine.Plugins;

[assembly: PluginMetadata("Rocket.PermissionLink", Author = "OpenMod", DisplayName = "Rocket Permission Link")]

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

        protected override async UniTask OnLoadAsync()
        {
            await base.OnLoadAsync();
            await UniTask.SwitchToMainThread();

            m_PermissionProvider = ActivatorUtilities.CreateInstance<OpenModPermissionsProvider>(m_ServiceProvider);
            m_PermissionProvider.Install();
        }

        protected override async UniTask OnUnloadAsync()
        {
            await base.OnUnloadAsync();
            await UniTask.SwitchToMainThread();
            m_PermissionProvider.Dispose();
        }
    }
}
