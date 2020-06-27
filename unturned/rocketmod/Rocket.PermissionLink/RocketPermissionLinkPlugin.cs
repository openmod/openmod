using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.Core.Plugins;
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

        protected override Task OnLoadAsync()
        {
            m_PermissionProvider = ActivatorUtilities.CreateInstance<OpenModPermissionsProvider>(m_ServiceProvider);
            m_PermissionProvider.Install();
            return base.OnLoadAsync();
        }

        protected override Task OnUnloadAsync()
        {
            m_PermissionProvider.Dispose();
            return base.OnUnloadAsync();
        }
    }
}
