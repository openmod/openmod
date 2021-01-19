using System;
using System.Diagnostics.CodeAnalysis;
using OpenMod.API.Eventing;
using OpenMod.Core.Eventing;
using OpenMod.Unturned.RocketMod.Events;
using SDG.Unturned;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.Unturned.Configuration;
using OpenMod.Unturned.RocketMod.Permissions;

#pragma warning disable IDE0079 // Remove unnecessary suppression
namespace OpenMod.Unturned.RocketMod
{
    public class RocketModLoadedEventListener : IEventListener<RocketModReadyEvent>
    {
        private readonly IServiceProvider m_ServiceProvider;
        private readonly IRocketModComponent m_RocketModComponent;
        private readonly IOpenModUnturnedConfiguration m_UnturnedConfiguration;
        private RocketModPermissionProxyProvider m_PermissionProxyProvider;

        public RocketModLoadedEventListener(
            IRocketModComponent rocketModComponent, 
            IOpenModUnturnedConfiguration unturnedConfiguration)
        {
            m_ServiceProvider = rocketModComponent.LifetimeScope.Resolve<IServiceProvider>();
            m_RocketModComponent = rocketModComponent;
            m_UnturnedConfiguration = unturnedConfiguration;
        }

        [EventListener]
        public Task HandleEventAsync(object sender, RocketModReadyEvent @event)
        {
            var permissionSystem = m_UnturnedConfiguration.Configuration
                .GetSection("rocketmodIntegration:permissionSystem")
                .Get<string>();

            if (permissionSystem.Equals("OpenMod", StringComparison.OrdinalIgnoreCase))
            {
                m_PermissionProxyProvider = ActivatorUtilities.CreateInstance<RocketModPermissionProxyProvider>(m_ServiceProvider);
                m_PermissionProxyProvider.Install();
            }

            RemoveRocketCommandListeners();
            return Task.CompletedTask;
        }

        [SuppressMessage("ReSharper", "DelegateSubtraction")]
        private void RemoveRocketCommandListeners()
        {
            var commandWindowInputedInvocationList = CommandWindow.onCommandWindowInputted.GetInvocationList();
            foreach (var @delegate in commandWindowInputedInvocationList
                .Where(IsRocketModDelegate))
            {
                CommandWindow.onCommandWindowInputted -= (CommandWindowInputted)@delegate;
            }

            var checkPermissionsList = ChatManager.onCheckPermissions.GetInvocationList();
            foreach (var @delegate in checkPermissionsList
                .Where(IsRocketModDelegate))
            {
                ChatManager.onCheckPermissions -= (CheckPermissions)@delegate;
            }
        }

        private bool IsRocketModDelegate(Delegate @delegate)
        {
            var methodInfo = @delegate.GetMethodInfo();
            var assembly = methodInfo?.DeclaringType?.Assembly;
            return RocketModIntegration.IsRocketModAssembly(assembly);
        }
    }
}