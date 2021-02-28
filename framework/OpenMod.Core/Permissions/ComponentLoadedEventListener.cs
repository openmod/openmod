using JetBrains.Annotations;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Permissions;
using OpenMod.Core.Events;
using OpenMod.Core.Plugins.Events;
using System.Reflection;
using System.Threading.Tasks;

namespace OpenMod.Core.Permissions
{
    [OpenModInternal]
    [UsedImplicitly]
    internal class ComponentLoadedEventListener :
        IEventListener<OpenModInitializedEvent>,
        IEventListener<PluginLoadedEvent>
    {
        private readonly IRuntime m_Runtime;
        private readonly IPermissionRegistry m_PermissionRegistry;

        public ComponentLoadedEventListener(IRuntime runtime,
            IPermissionRegistry permissionRegistry)
        {
            m_Runtime = runtime;
            m_PermissionRegistry = permissionRegistry;
        }

        private void RegisterComponentPermissions(IOpenModComponent component, Assembly? assembly = null)
        {
            assembly ??= component.GetType().Assembly;

            var attribs = assembly.GetCustomAttributes<RegisterPermissionAttribute>();

            foreach (var attrib in attribs)
            {
                m_PermissionRegistry.RegisterPermission(component, attrib.Permission, attrib.Description,
                    attrib.DefaultGrant);
            }
        }

        public Task HandleEventAsync(object? sender, OpenModInitializedEvent @event)
        {
            // OpenMod.Core
            RegisterComponentPermissions(m_Runtime, GetType().Assembly);
            
            RegisterComponentPermissions(@event.Host);

            return Task.CompletedTask;
        }

        public Task HandleEventAsync(object? sender, PluginLoadedEvent @event)
        {
            RegisterComponentPermissions(@event.Plugin);

            return Task.CompletedTask;
        }
    }
}
