using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenMod.API.Ioc;
using OpenMod.API.Permissions;
using OpenMod.API.Prioritization;
using OpenMod.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenMod.API;

namespace OpenMod.Core.Permissions
{
    [OpenModInternal]
    [UsedImplicitly]
    [ServiceImplementation(Priority = Priority.Lowest, Lifetime = ServiceLifetime.Singleton)]
    public class PermissionChecker : IPermissionChecker, IAsyncDisposable
    {
        private bool m_IsDisposing;

        private readonly IServiceProvider m_ServiceProvider;
        private readonly IPermissionRegistry m_PermissionRegistry;
        private readonly IOptions<PermissionCheckerOptions> m_Options;
        private readonly List<IPermissionStore> m_PermissionSources;
        private readonly List<IPermissionCheckProvider> m_PermissionCheckProviders;
        private readonly ILogger<PermissionChecker> m_Logger;
        public PermissionChecker(
            IServiceProvider serviceProvider,
            IPermissionRegistry permissionRegistry,
            IOptions<PermissionCheckerOptions> options,
            ILogger<PermissionChecker> logger)
        {
            m_ServiceProvider = serviceProvider;
            m_PermissionRegistry = permissionRegistry;
            m_Options = options;
            m_Logger = logger;
            m_PermissionSources = new List<IPermissionStore>();
            m_PermissionCheckProviders = new List<IPermissionCheckProvider>();
        }

        public IReadOnlyCollection<IPermissionCheckProvider> PermissionCheckProviders => m_PermissionCheckProviders;

        public IReadOnlyCollection<IPermissionStore> PermissionStores => m_PermissionSources;

        public async Task<PermissionGrantResult> CheckPermissionAsync(IPermissionActor actor, string permission)
        {
            var registration = m_PermissionRegistry.FindPermission(permission);
            if (registration == null)
            {
                throw new Exception($"Permission is not registered: {permission}");
            }

            foreach (var provider in m_PermissionCheckProviders.Where(c => c.SupportsActor(actor)))
            {
                var result = await provider.CheckPermissionAsync(actor, permission);
                m_Logger.LogDebug($"{actor.Type}/{actor.DisplayName} permission check result for \"{permission}\" ({provider.GetType().Name}): {result}");
             
                if (result != PermissionGrantResult.Default)
                {
                    return result;
                }
            }

            m_Logger.LogDebug($"{actor.Type}/{actor.DisplayName} permission check \"{permission}\" returning default");
            return registration.DefaultGrant;
        }

        public Task InitAsync()
        {
            foreach (var permissionSourceType in m_Options.Value.PermissionSources)
            {
                m_PermissionSources.Add((IPermissionStore)ActivatorUtilities.CreateInstance(m_ServiceProvider, permissionSourceType));
            }

            foreach (var permissionCheckProviderType in m_Options.Value.PermissionCheckProviders)
            {
                m_PermissionCheckProviders.Add((IPermissionCheckProvider)ActivatorUtilities.CreateInstance(m_ServiceProvider, permissionCheckProviderType));
            }

            return Task.CompletedTask;
        }

        public async ValueTask DisposeAsync()
        {
            if (m_IsDisposing)
            {
                return;
            }
            m_IsDisposing = true;

            await m_PermissionCheckProviders.DisposeAllAsync();
            await m_PermissionSources.DisposeAllAsync();
        }
    }
}